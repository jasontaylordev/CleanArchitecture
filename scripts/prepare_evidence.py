#!/usr/bin/env python3
"""Collect immutable, allowlisted architecture evidence from GitHub."""

from __future__ import annotations

import argparse
import base64
import datetime as dt
import fnmatch
import json
import os
import re
import shutil
import ssl
import stat
import subprocess
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path, PurePosixPath
from typing import Any, Iterable, NoReturn

import yaml


API_ORIGIN = "https://api.github.com"
GIT_ORIGIN = "https://github.com"
MAX_FILE_BYTES = 1_048_576
MAX_SOURCE_BYTES = 25 * 1_048_576

DENIED_PATH_PATTERNS = (
    ".env",
    ".env.*",
    "**/.env",
    "**/.env.*",
    "**/*.pem",
    "**/*.key",
    "**/*.p12",
    "**/*.pfx",
    "**/id_rsa",
    "**/id_ed25519",
    "**/*secret*",
    "**/.git/**",
    "**/node_modules/**",
    "**/vendor/**",
)

SECRET_PATTERNS = (
    re.compile(
        rb"-----BEGIN [A-Z ]*PRIVATE KEY-----.*?"
        rb"-----END [A-Z ]*PRIVATE KEY-----",
        re.DOTALL,
    ),
    re.compile(rb"\bgh[pousr]_[A-Za-z0-9_]{20,}\b"),
    re.compile(rb"\bgithub_pat_[A-Za-z0-9_]{20,}\b"),
    re.compile(rb"\bAKIA[0-9A-Z]{16}\b"),
    re.compile(rb"\bAIza[0-9A-Za-z_-]{30,}\b"),
)

FULL_SHA_PATTERN = re.compile(r"^[0-9a-f]{40}$")
CHANGE_REFERENCE_PATTERN = re.compile(
    r"^([A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+)#([0-9]+)$"
)


class EvidenceError(RuntimeError):
    """Raised when evidence collection cannot complete safely."""


def load_yaml(path: Path) -> dict[str, Any]:
    """Load a YAML mapping with PyYAML safe loading."""
    loaded = yaml.safe_load(path.read_text(encoding="utf-8"))

    if not isinstance(loaded, dict):
        raise EvidenceError(f"{path} must contain a YAML object")

    return loaded


def utc_now() -> str:
    """Return an ISO 8601 UTC timestamp."""
    return (
        dt.datetime.now(dt.timezone.utc)
        .replace(microsecond=0)
        .isoformat()
        .replace("+00:00", "Z")
    )


def base64url(value: bytes) -> str:
    """Create unpadded URL-safe base64."""
    return base64.urlsafe_b64encode(value).rstrip(b"=").decode("ascii")


def der_length(length: int) -> bytes:
    """Encode a DER length."""
    if length < 128:
        return bytes((length,))

    encoded = length.to_bytes((length.bit_length() + 7) // 8, "big")
    return bytes((0x80 | len(encoded),)) + encoded


def der_integer(value: int) -> bytes:
    """Encode a positive DER integer."""
    if value < 0:
        raise EvidenceError("RSA integer cannot be negative")

    encoded = value.to_bytes(max(1, (value.bit_length() + 7) // 8), "big")
    if encoded[0] & 0x80:
        encoded = b"\x00" + encoded

    return b"\x02" + der_length(len(encoded)) + encoded


def der_sequence(*items: bytes) -> bytes:
    """Encode a DER sequence."""
    payload = b"".join(items)
    return b"\x30" + der_length(len(payload)) + payload


def rsa_public_key_pkcs1_der(modulus: int, exponent: int) -> bytes:
    """Build a PKCS#1 RSA public-key DER sequence."""
    return der_sequence(
        der_integer(modulus),
        der_integer(exponent),
    )


def load_rsa_private_key(private_key_text: str) -> Any:
    """
    Load a PEM RSA private key with the cryptography package.

    GitHub-hosted workflows install cryptography as a pinned dependency.
    """
    try:
        from cryptography.hazmat.primitives import serialization
        from cryptography.hazmat.primitives.asymmetric.rsa import RSAPrivateKey
    except ImportError as error:
        raise EvidenceError(
            "cryptography is required to sign the GitHub App JWT"
        ) from error

    key = serialization.load_pem_private_key(
        private_key_text.encode("utf-8"),
        password=None,
    )

    if not isinstance(key, RSAPrivateKey):
        raise EvidenceError("GitHub App private key is not an RSA key")

    return key


def app_jwt() -> str:
    """Create a short-lived GitHub App JWT."""
    try:
        app_id = os.environ["ARCH_EVIDENCE_APP_ID"]
        private_key_text = os.environ["ARCH_EVIDENCE_APP_PRIVATE_KEY"]
    except KeyError as error:
        raise EvidenceError(f"missing required environment variable {error}") from error

    try:
        from cryptography.hazmat.primitives import hashes
        from cryptography.hazmat.primitives.asymmetric import padding
    except ImportError as error:
        raise EvidenceError(
            "cryptography is required to sign the GitHub App JWT"
        ) from error

    now = int(time.time())
    header = base64url(
        json.dumps(
            {"alg": "RS256", "typ": "JWT"},
            separators=(",", ":"),
        ).encode("utf-8")
    )
    payload = base64url(
        json.dumps(
            {
                "iat": now - 60,
                "exp": now + 540,
                "iss": app_id,
            },
            separators=(",", ":"),
        ).encode("utf-8")
    )

    signing_input = f"{header}.{payload}".encode("ascii")
    private_key = load_rsa_private_key(private_key_text)
    signature = private_key.sign(
        signing_input,
        padding.PKCS1v15(),
        hashes.SHA256(),
    )

    return f"{header}.{payload}.{base64url(signature)}"


def api_request(
    method: str,
    path: str,
    token: str,
    body: dict[str, Any] | None = None,
) -> Any:
    """Call the GitHub REST API over HTTPS."""
    if not path.startswith("/"):
        raise EvidenceError("invalid GitHub API path")

    url = f"{API_ORIGIN}{path}"
    data = None

    if body is not None:
        data = json.dumps(body).encode("utf-8")

    request = urllib.request.Request(
        url=url,
        data=data,
        method=method.upper(),
        headers={
            "Accept": "application/vnd.github+json",
            "X-GitHub-Api-Version": "2022-11-28",
            "User-Agent": "architecture-evidence-v1",
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json",
        },
    )

    try:
        with urllib.request.urlopen(
            request,
            timeout=60,
            context=ssl.create_default_context(),
        ) as response:
            payload = response.read()
    except urllib.error.HTTPError as error:
        raise EvidenceError(
            f"GitHub API {method.upper()} {path} failed "
            f"with HTTP {error.code}"
        ) from error
    except urllib.error.URLError as error:
        raise EvidenceError(
            f"GitHub API {method.upper()} {path} failed: {error.reason}"
        ) from error

    if not payload:
        return {}

    return json.loads(payload.decode("utf-8"))


def split_repository(repository: str) -> tuple[str, str]:
    """Split and validate an owner/repository name."""
    parts = repository.split("/", 1)

    if len(parts) != 2 or not all(parts):
        raise EvidenceError(f"invalid repository {repository!r}")

    return parts[0], parts[1]


def quote_path_component(value: str) -> str:
    """Quote one REST path component."""
    return urllib.parse.quote(value, safe="")


def repository_token(repository: str) -> str:
    """Mint a repository-restricted installation token."""
    owner, name = split_repository(repository)
    jwt = app_jwt()

    installation = api_request(
        "GET",
        (
            f"/repos/{quote_path_component(owner)}/"
            f"{quote_path_component(name)}/installation"
        ),
        token=jwt,
    )

    installation_id = installation.get("id")
    if not isinstance(installation_id, int):
        raise EvidenceError(
            f"GitHub App installation not found for {repository}"
        )

    response = api_request(
        "POST",
        f"/app/installations/{installation_id}/access_tokens",
        token=jwt,
        body={
            "repositories": [name],
            "permissions": {
                "contents": "read",
                "issues": "read",
                "pull_requests": "read",
            },
        },
    )

    token = response.get("token")
    if not isinstance(token, str) or not token:
        raise EvidenceError(
            f"GitHub did not return an installation token for {repository}"
        )

    return token


def resolve_commit(repository: str, ref: str, token: str) -> str:
    """Resolve a configured ref or supplied SHA to a full commit SHA."""
    owner, name = split_repository(repository)
    encoded_ref = quote_path_component(ref)

    result = api_request(
        "GET",
        (
            f"/repos/{quote_path_component(owner)}/"
            f"{quote_path_component(name)}/commits/{encoded_ref}"
        ),
        token=token,
    )

    sha = result.get("sha")
    if not isinstance(sha, str) or not FULL_SHA_PATTERN.fullmatch(sha):
        raise EvidenceError(
            f"GitHub returned a non-commit SHA for {repository}@{ref}"
        )

    return sha


def run_git(
    arguments: Iterable[str],
    *,
    environment: dict[str, str] | None = None,
    text: bool = False,
) -> bytes | str:
    """Run Git without invoking a shell."""
    command = ["git", *arguments]
    process_environment = os.environ.copy()

    if environment:
        process_environment.update(environment)

    completed = subprocess.run(
        command,
        check=False,
        capture_output=True,
        env=process_environment,
        text=text,
    )

    if completed.returncode != 0:
        stderr = (
            completed.stderr
            if isinstance(completed.stderr, str)
            else completed.stderr.decode("utf-8", errors="replace")
        )
        last_line = stderr.strip().splitlines()[-1:] or ["unknown error"]
        raise EvidenceError(
            f"git {arguments[0]} failed: {last_line[0]}"
        )

    return completed.stdout


def create_askpass(directory: Path) -> Path:
    """Create a non-interactive Git askpass helper."""
    if os.name == "nt":
        askpass = directory / "askpass.cmd"
        askpass.write_text(
            "@echo off\r\n"
            'echo %ARCH_REPOSITORY_TOKEN%\r\n',
            encoding="utf-8",
        )
    else:
        askpass = directory / "askpass.sh"
        askpass.write_text(
            "#!/bin/sh\n"
            'case "$1" in\n'
            "  *Username*) printf '%s\\n' 'x-access-token' ;;\n"
            "  *) printf '%s\\n' \"$ARCH_REPOSITORY_TOKEN\" ;;\n"
            "esac\n",
            encoding="utf-8",
        )
        askpass.chmod(
            askpass.stat().st_mode | stat.S_IXUSR
        )

    return askpass


def checkout_repository(
    repository: str,
    sha: str,
    token: str,
) -> tuple[tempfile.TemporaryDirectory[str], Path, dict[str, str]]:
    """Fetch exactly one commit into a temporary detached checkout."""
    temporary = tempfile.TemporaryDirectory(prefix="architecture-source-")
    directory = Path(temporary.name)
    checkout = directory / "repository"
    checkout.mkdir()

    askpass = create_askpass(directory)
    environment = {
        "GIT_ASKPASS": str(askpass),
        "GIT_TERMINAL_PROMPT": "0",
        "ARCH_REPOSITORY_TOKEN": token,
    }

    try:
        run_git(["init", "--quiet", str(checkout)], environment=environment)
        run_git(
            [
                "-C",
                str(checkout),
                "remote",
                "add",
                "origin",
                f"{GIT_ORIGIN}/{repository}.git",
            ],
            environment=environment,
        )
        run_git(
            [
                "-C",
                str(checkout),
                "fetch",
                "--quiet",
                "--depth=1",
                "origin",
                sha,
            ],
            environment=environment,
        )
        run_git(
            [
                "-C",
                str(checkout),
                "checkout",
                "--quiet",
                "--detach",
                "FETCH_HEAD",
            ],
            environment=environment,
        )

        actual = str(
            run_git(
                ["-C", str(checkout), "rev-parse", "HEAD"],
                environment=environment,
                text=True,
            )
        ).strip()

        if actual != sha:
            raise EvidenceError(
                f"checked out {actual}, expected {sha}"
            )

        return temporary, checkout, environment
    except Exception:
        temporary.cleanup()
        raise


def normalize_git_path(path: str) -> str:
    """Validate a repository-relative POSIX path."""
    pure = PurePosixPath(path)

    if pure.is_absolute() or ".." in pure.parts:
        raise EvidenceError(f"unsafe repository path {path!r}")

    normalized = pure.as_posix()

    if normalized in ("", "."):
        raise EvidenceError(f"invalid repository path {path!r}")

    return normalized


def glob_match(pattern: str, path: str) -> bool:
    """Match repository paths with slash-aware glob behavior."""
    pattern_path = PurePosixPath(pattern)
    path_value = PurePosixPath(path)

    if path_value.match(pattern):
        return True

    if pattern.startswith("**/"):
        return path_value.match(pattern[3:])

    if "/" not in pattern:
        return fnmatch.fnmatchcase(path_value.name, pattern)

    return False


def selected_path(
    path: str,
    include_patterns: list[str],
    exclude_patterns: list[str],
) -> bool:
    """Apply built-in denials and configured include/exclude rules."""
    normalized = normalize_git_path(path)

    if any(glob_match(pattern, normalized) for pattern in DENIED_PATH_PATTERNS):
        return False

    if not any(glob_match(pattern, normalized) for pattern in include_patterns):
        return False

    if any(glob_match(pattern, normalized) for pattern in exclude_patterns):
        return False

    return True


def redact(content: bytes) -> bytes:
    """Redact obvious credential formats before model exposure."""
    redacted = content

    for pattern in SECRET_PATTERNS:
        redacted = pattern.sub(b"[REDACTED-SECRET]", redacted)

    return redacted


def safe_destination(root: Path, relative_path: str) -> Path:
    """Resolve a destination while preventing path traversal."""
    resolved_root = root.resolve()
    target = (resolved_root / Path(*PurePosixPath(relative_path).parts)).resolve()

    try:
        target.relative_to(resolved_root)
    except ValueError as error:
        raise EvidenceError(
            f"unsafe evidence destination for {relative_path!r}"
        ) from error

    return target


def collect_repository(
    source: dict[str, Any],
    sha: str,
    token: str,
    output_root: Path,
    *,
    namespace: str | None = None,
    restrict_paths: set[str] | None = None,
) -> dict[str, Any]:
    """Materialize allowlisted blobs from one immutable commit."""
    repository = source["repository"]
    include_patterns = list(source["include"])
    exclude_patterns = list(source["exclude"])
    repository_directory = repository.replace("/", "__")

    destination = output_root
    if namespace:
        destination = destination / namespace
    destination = destination / repository_directory
    destination.mkdir(parents=True, exist_ok=True)

    included: list[dict[str, Any]] = []
    skipped: list[dict[str, Any]] = []
    total_bytes = 0

    temporary, checkout, environment = checkout_repository(
        repository,
        sha,
        token,
    )

    try:
        listing = run_git(
            [
                "-C",
                str(checkout),
                "ls-tree",
                "-r",
                "-z",
                "--full-tree",
                sha,
            ],
            environment=environment,
        )

        if not isinstance(listing, bytes):
            raise EvidenceError("unexpected text Git tree output")

        for raw_entry in listing.split(b"\x00"):
            if not raw_entry:
                continue

            try:
                raw_metadata, raw_path = raw_entry.split(b"\t", 1)
                mode, object_type, object_sha = (
                    raw_metadata.decode("ascii").split(" ", 2)
                )
                path = raw_path.decode("utf-8")
            except (ValueError, UnicodeDecodeError) as error:
                raise EvidenceError(
                    "could not parse Git tree entry safely"
                ) from error

            normalized_path = normalize_git_path(path)

            if not selected_path(
                normalized_path,
                include_patterns,
                exclude_patterns,
            ):
                continue

            if restrict_paths is not None and normalized_path not in restrict_paths:
                continue

            if object_type != "blob" or mode not in ("100644", "100755"):
                skipped.append(
                    {
                        "path": normalized_path,
                        "reason": "not_regular_file",
                    }
                )
                continue

            raw_size = run_git(
                [
                    "-C",
                    str(checkout),
                    "cat-file",
                    "-s",
                    object_sha,
                ],
                environment=environment,
                text=True,
            )
            size = int(str(raw_size).strip())

            if size > MAX_FILE_BYTES:
                skipped.append(
                    {
                        "path": normalized_path,
                        "reason": "oversized",
                        "bytes": size,
                    }
                )
                continue

            if total_bytes + size > MAX_SOURCE_BYTES:
                skipped.append(
                    {
                        "path": normalized_path,
                        "reason": "source_size_limit",
                        "bytes": size,
                    }
                )
                continue

            content = run_git(
                [
                    "-C",
                    str(checkout),
                    "cat-file",
                    "blob",
                    object_sha,
                ],
                environment=environment,
            )

            if not isinstance(content, bytes):
                raise EvidenceError("unexpected text Git blob output")

            if b"\x00" in content:
                skipped.append(
                    {
                        "path": normalized_path,
                        "reason": "binary",
                    }
                )
                continue

            target = safe_destination(destination, normalized_path)
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(redact(content))

            total_bytes += size
            included.append(
                {
                    "path": normalized_path,
                    "bytes": size,
                }
            )
    finally:
        environment["ARCH_REPOSITORY_TOKEN"] = ""
        token = ""
        temporary.cleanup()

    return {
        "repository": repository,
        "configured_ref": source["ref"],
        "role": source["role"],
        "commit": sha,
        "namespace": namespace,
        "files": included,
        "skipped": skipped,
    }


def paginated(
    repository: str,
    endpoint: str,
    token: str,
) -> list[dict[str, Any]]:
    """Read a paginated GitHub REST collection."""
    owner, name = split_repository(repository)
    results: list[dict[str, Any]] = []
    page = 1

    while True:
        path = (
            f"/repos/{quote_path_component(owner)}/"
            f"{quote_path_component(name)}/{endpoint}"
            f"?per_page=100&page={page}"
        )
        batch = api_request("GET", path, token=token)

        if not isinstance(batch, list):
            raise EvidenceError(
                f"GitHub endpoint {endpoint} did not return a list"
            )

        if not batch:
            break

        results.extend(batch)

        if len(batch) < 100:
            break

        page += 1

    return results


def write_json(path: Path, value: Any) -> None:
    """Write deterministic, readable UTF-8 JSON."""
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        json.dumps(value, indent=2, sort_keys=False) + "\n",
        encoding="utf-8",
    )


def prepare_output(output: Path) -> None:
    """Replace an output directory safely."""
    if output.exists():
        shutil.rmtree(output)

    output.mkdir(parents=True)


def bootstrap(scope_path: Path, output: Path) -> None:
    """Collect all configured bootstrap sources."""
    scope = load_yaml(scope_path)
    prepare_output(output)
    source_results: list[dict[str, Any]] = []

    for source in scope["sources"]:
        token = repository_token(source["repository"])

        try:
            sha = resolve_commit(
                source["repository"],
                source["ref"],
                token,
            )
            source_results.append(
                collect_repository(
                    source,
                    sha,
                    token,
                    output,
                )
            )
        finally:
            token = ""

    write_json(
        output / "manifest.json",
        {
            "version": 1,
            "mode": "bootstrap",
            "generated_at": utc_now(),
            "sources": source_results,
        },
    )


def sanitize_text(value: Any) -> str:
    """Convert an API value to redacted text."""
    text = "" if value is None else str(value)
    return redact(text.encode("utf-8")).decode("utf-8", errors="replace")


def update(
    scope_path: Path,
    reference: str,
    output: Path,
) -> None:
    """Collect one issue or pull request and relevant immutable files."""
    match = CHANGE_REFERENCE_PATTERN.fullmatch(reference)

    if not match:
        raise EvidenceError(
            f"invalid change reference {reference!r}"
        )

    repository = match.group(1)
    number = int(match.group(2))
    scope = load_yaml(scope_path)

    source = next(
        (
            candidate
            for candidate in scope["sources"]
            if candidate["repository"] == repository
        ),
        None,
    )

    if source is None:
        raise EvidenceError(
            f"{repository} is not present in bootstrap scope"
        )

    prepare_output(output)
    token = repository_token(repository)

    try:
        owner, name = split_repository(repository)
        issue = api_request(
            "GET",
            (
                f"/repos/{quote_path_component(owner)}/"
                f"{quote_path_component(name)}/issues/{number}"
            ),
            token=token,
        )

        sanitized_issue = {
            "number": issue.get("number"),
            "title": sanitize_text(issue.get("title")),
            "body": sanitize_text(issue.get("body")),
            "state": issue.get("state"),
            "user": (issue.get("user") or {}).get("login"),
            "html_url": issue.get("html_url"),
            "pull_request": issue.get("pull_request") is not None,
        }

        comments = [
            {
                "user": (comment.get("user") or {}).get("login"),
                "created_at": comment.get("created_at"),
                "body": sanitize_text(comment.get("body")),
            }
            for comment in paginated(
                repository,
                f"issues/{number}/comments",
                token,
            )
        ]

        write_json(
            output / "change.json",
            {
                "issue": sanitized_issue,
                "comments": comments,
            },
        )

        source_results: list[dict[str, Any]] = []
        metadata_files = ["change.json"]

        if issue.get("pull_request") is not None:
            pull = api_request(
                "GET",
                (
                    f"/repos/{quote_path_component(owner)}/"
                    f"{quote_path_component(name)}/pulls/{number}"
                ),
                token=token,
            )
            files = paginated(
                repository,
                f"pulls/{number}/files",
                token,
            )

            changed_paths = {
                normalize_git_path(str(item["filename"]))
                for item in files
            }

            base_sha = (pull.get("base") or {}).get("sha")
            head_sha = (pull.get("head") or {}).get("sha")

            if (
                not isinstance(base_sha, str)
                or not FULL_SHA_PATTERN.fullmatch(base_sha)
            ):
                raise EvidenceError("invalid pull request base SHA")

            if (
                not isinstance(head_sha, str)
                or not FULL_SHA_PATTERN.fullmatch(head_sha)
            ):
                raise EvidenceError("invalid pull request head SHA")

            write_json(
                output / "pull-request.json",
                {
                    "number": number,
                    "base_sha": base_sha,
                    "head_sha": head_sha,
                    "changed_files": [
                        {
                            "filename": item.get("filename"),
                            "status": item.get("status"),
                            "additions": item.get("additions"),
                            "deletions": item.get("deletions"),
                            "patch": sanitize_text(item.get("patch")),
                        }
                        for item in files
                    ],
                },
            )
            metadata_files.append("pull-request.json")

            source_results.append(
                collect_repository(
                    source,
                    base_sha,
                    token,
                    output,
                    namespace="base",
                    restrict_paths=changed_paths,
                )
            )
            source_results.append(
                collect_repository(
                    source,
                    head_sha,
                    token,
                    output,
                    namespace="head",
                    restrict_paths=changed_paths,
                )
            )

        write_json(
            output / "manifest.json",
            {
                "version": 1,
                "mode": "update",
                "reference": reference,
                "generated_at": utc_now(),
                "sources": source_results,
                "metadata_files": metadata_files,
            },
        )
    finally:
        token = ""


SOURCE_ROW_PATTERN = re.compile(
    r"^\|\s*SRC-[0-9]+\s*\|\s*"
    r"([^| ]+/[^| ]+)\s*\|\s*"
    r"(primary|supporting)\s*\|\s*"
    r"([^|]+?)\s*\|\s*"
    r"([0-9a-f]{40})\s*\|$"
)


def verification_rows(
    architecture_path: Path,
) -> list[dict[str, str]]:
    """Read declared source rows from the authoritative document."""
    rows: list[dict[str, str]] = []

    for line in architecture_path.read_text(
        encoding="utf-8"
    ).splitlines():
        match = SOURCE_ROW_PATTERN.fullmatch(line)

        if not match:
            continue

        rows.append(
            {
                "repository": match.group(1),
                "role": match.group(2),
                "ref": match.group(3).strip(),
                "sha": match.group(4),
            }
        )

    return rows


def verify(
    scope_path: Path,
    architecture_path: Path,
    output: Path,
) -> None:
    """Collect evidence for the exact commits declared in architecture.md."""
    prepare_output(output)

    if not (
        os.environ.get("ARCH_EVIDENCE_APP_ID")
        and os.environ.get("ARCH_EVIDENCE_APP_PRIVATE_KEY")
    ):
        write_json(
            output / "manifest.json",
            {
                "version": 1,
                "mode": "verify",
                "evidence_available": False,
                "reason": (
                    "GitHub App credentials were not available "
                    "to this workflow run"
                ),
                "sources": [],
            },
        )
        return

    try:
        scope = load_yaml(scope_path)
        source_by_repository = {
            source["repository"]: source
            for source in scope["sources"]
        }
        source_results: list[dict[str, Any]] = []

        for row in verification_rows(architecture_path):
            repository = row["repository"]

            if repository not in source_by_repository:
                raise EvidenceError(
                    f"{repository} is declared in architecture.md "
                    "but absent from scope"
                )

            source = source_by_repository[repository]
            token = repository_token(repository)

            try:
                resolved = resolve_commit(
                    repository,
                    row["sha"],
                    token,
                )

                if resolved != row["sha"]:
                    raise EvidenceError(
                        f"commit verification mismatch for {repository}"
                    )

                source_results.append(
                    collect_repository(
                        source,
                        row["sha"],
                        token,
                        output,
                    )
                )
            finally:
                token = ""

        write_json(
            output / "manifest.json",
            {
                "version": 1,
                "mode": "verify",
                "evidence_available": True,
                "generated_at": utc_now(),
                "sources": source_results,
            },
        )
    except Exception as error:
        if os.environ.get("ARCH_EVIDENCE_OPTIONAL") != "1":
            raise

        prepare_output(output)
        write_json(
            output / "manifest.json",
            {
                "version": 1,
                "mode": "verify",
                "evidence_available": False,
                "reason": str(error),
                "sources": [],
            },
        )


def build_parser() -> argparse.ArgumentParser:
    """Build the command-line parser."""
    parser = argparse.ArgumentParser(
        description=__doc__,
    )
    subparsers = parser.add_subparsers(
        dest="command",
        required=True,
    )

    bootstrap_parser = subparsers.add_parser("bootstrap")
    bootstrap_parser.add_argument("scope", type=Path)
    bootstrap_parser.add_argument("output", type=Path)

    update_parser = subparsers.add_parser("update")
    update_parser.add_argument("scope", type=Path)
    update_parser.add_argument("reference")
    update_parser.add_argument("output", type=Path)

    verify_parser = subparsers.add_parser("verify")
    verify_parser.add_argument("scope", type=Path)
    verify_parser.add_argument("architecture", type=Path)
    verify_parser.add_argument("output", type=Path)

    return parser


def main() -> int:
    """Program entry point."""
    parser = build_parser()
    arguments = parser.parse_args()

    try:
        if arguments.command == "bootstrap":
            bootstrap(arguments.scope, arguments.output)
        elif arguments.command == "update":
            update(
                arguments.scope,
                arguments.reference,
                arguments.output,
            )
        elif arguments.command == "verify":
            verify(
                arguments.scope,
                arguments.architecture,
                arguments.output,
            )
        else:
            parser.error("unsupported command")
    except (
        EvidenceError,
        OSError,
        ValueError,
        KeyError,
        json.JSONDecodeError,
        yaml.YAMLError,
    ) as error:
        print(f"evidence collection failed: {error}", file=os.sys.stderr)
        return 1

    return 0


if __name__ == "__main__":
    raise SystemExit(main())