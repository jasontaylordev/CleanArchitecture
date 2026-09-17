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
import sys
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path, PurePosixPath
from typing import Any, Iterable

import yaml


API_ORIGIN = "https://api.github.com"
GIT_ORIGIN = "https://github.com"

ACCESS_LOCAL = "local"
ACCESS_GITHUB_APP = "github_app"

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
    "**/.architecture-local-source/**",
    "**/.architecture-work/**",
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

SOURCE_ROW_PATTERN = re.compile(
    r"^\|\s*SRC-[0-9]+\s*\|\s*"
    r"([^| ]+/[^| ]+)\s*\|\s*"
    r"(primary|supporting)\s*\|\s*"
    r"([^|]+?)\s*\|\s*"
    r"([0-9a-f]{40})\s*\|$"
)


class EvidenceError(RuntimeError):
    """Raised when evidence collection cannot complete safely."""


def load_yaml(path: Path) -> dict[str, Any]:
    """Load a YAML mapping using safe YAML parsing."""
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


def split_repository(repository: str) -> tuple[str, str]:
    """Split and validate an owner/repository name."""
    parts = repository.split("/", 1)

    if len(parts) != 2 or not all(parts):
        raise EvidenceError(f"invalid repository {repository!r}")

    return parts[0], parts[1]


def quote_path_component(value: str) -> str:
    """Quote a REST API path component."""
    return urllib.parse.quote(value, safe="")


def validate_scope_runtime(scope: dict[str, Any]) -> None:
    """Apply environment-dependent scope rules not expressible in JSON Schema."""
    sources = scope.get("sources")

    if not isinstance(sources, list) or not sources:
        raise EvidenceError("scope must contain at least one source")

    local_sources = [
        source
        for source in sources
        if source.get("access") == ACCESS_LOCAL
    ]

    if len(local_sources) > 1:
        raise EvidenceError(
            "scope may contain at most one source with access: local"
        )

    current_repository = os.environ.get("GITHUB_REPOSITORY")

    if local_sources and current_repository:
        declared_repository = local_sources[0].get("repository", "")

        if declared_repository.casefold() != current_repository.casefold():
            raise EvidenceError(
                "local source repository does not match GITHUB_REPOSITORY: "
                f"declared {declared_repository!r}, "
                f"running in {current_repository!r}"
            )


def find_source(
    scope: dict[str, Any],
    repository: str,
) -> dict[str, Any]:
    """Find one configured source repository."""
    for source in scope["sources"]:
        if source["repository"].casefold() == repository.casefold():
            return source

    raise EvidenceError(
        f"{repository} is not present in bootstrap scope"
    )


def access_requires_github_app(access: str) -> bool:
    """Return whether an access mode requires GitHub App credentials."""
    if access == ACCESS_LOCAL:
        return False

    if access == ACCESS_GITHUB_APP:
        return True

    raise EvidenceError(f"unsupported source access mode {access!r}")


def bootstrap_access(scope_path: Path) -> str:
    """Report whether bootstrap requires GitHub App credentials."""
    scope = load_yaml(scope_path)
    validate_scope_runtime(scope)

    if any(
        access_requires_github_app(source["access"])
        for source in scope["sources"]
    ):
        return ACCESS_GITHUB_APP

    return ACCESS_LOCAL


def update_access(scope_path: Path, reference: str) -> str:
    """Report the access mode for an issue or pull-request reference."""
    match = CHANGE_REFERENCE_PATTERN.fullmatch(reference)

    if not match:
        raise EvidenceError(
            f"invalid change reference {reference!r}; "
            "expected owner/repository#number"
        )

    scope = load_yaml(scope_path)
    validate_scope_runtime(scope)
    source = find_source(scope, match.group(1))

    access_requires_github_app(source["access"])
    return source["access"]


def verification_rows(
    architecture_path: Path,
) -> list[dict[str, str]]:
    """Read source declarations from the authoritative document."""
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


def verification_access(
    scope_path: Path,
    architecture_path: Path,
) -> str:
    """Report whether verification requires GitHub App credentials."""
    scope = load_yaml(scope_path)
    validate_scope_runtime(scope)

    for row in verification_rows(architecture_path):
        source = find_source(scope, row["repository"])

        if access_requires_github_app(source["access"]):
            return ACCESS_GITHUB_APP

    return ACCESS_LOCAL


def normalize_private_key(private_key_text: str) -> str:
    """Normalize common GitHub secret formatting for a PEM private key."""
    normalized = private_key_text.strip()

    if "\\n" in normalized and "\n" not in normalized:
        normalized = normalized.replace("\\n", "\n")

    normalized = normalized.replace("\r\n", "\n")

    if normalized:
        normalized += "\n"

    return normalized


def base64url(value: bytes) -> str:
    """Create unpadded URL-safe base64."""
    return base64.urlsafe_b64encode(value).rstrip(b"=").decode("ascii")


def app_jwt() -> str:
    """Create a short-lived GitHub App JWT."""
    app_id = os.environ.get("ARCH_EVIDENCE_APP_ID", "").strip()
    private_key_text = normalize_private_key(
        os.environ.get("ARCH_EVIDENCE_APP_PRIVATE_KEY", "")
    )

    if not app_id:
        raise EvidenceError(
            "ARCH_EVIDENCE_APP_ID is required for a source using "
            "access: github_app"
        )

    if not private_key_text:
        raise EvidenceError(
            "ARCH_EVIDENCE_APP_PRIVATE_KEY is required for a source "
            "using access: github_app"
        )

    try:
        from cryptography.hazmat.primitives import hashes, serialization
        from cryptography.hazmat.primitives.asymmetric import padding
        from cryptography.hazmat.primitives.asymmetric.rsa import RSAPrivateKey
    except ImportError as error:
        raise EvidenceError(
            "cryptography is required for GitHub App authentication"
        ) from error

    try:
        private_key = serialization.load_pem_private_key(
            private_key_text.encode("utf-8"),
            password=None,
        )
    except (TypeError, ValueError) as error:
        raise EvidenceError(
            "ARCH_EVIDENCE_APP_PRIVATE_KEY is not a valid unencrypted "
            "GitHub App RSA PEM private key"
        ) from error

    if not isinstance(private_key, RSAPrivateKey):
        raise EvidenceError(
            "ARCH_EVIDENCE_APP_PRIVATE_KEY is not an RSA private key"
        )

    now = int(time.time())

    header = base64url(
        json.dumps(
            {
                "alg": "RS256",
                "typ": "JWT",
            },
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
    """Call the GitHub REST API."""
    if not path.startswith("/"):
        raise EvidenceError("invalid GitHub API path")

    if not token:
        raise EvidenceError(
            f"a GitHub token is required for API request {path}"
        )

    data = None

    if body is not None:
        data = json.dumps(body).encode("utf-8")

    request = urllib.request.Request(
        url=f"{API_ORIGIN}{path}",
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


def built_in_github_token() -> str:
    """Return the workflow's built-in same-repository GitHub token."""
    token = os.environ.get("ARCH_EVIDENCE_GITHUB_TOKEN", "").strip()

    if not token:
        token = os.environ.get("GITHUB_TOKEN", "").strip()

    if not token:
        raise EvidenceError(
            "ARCH_EVIDENCE_GITHUB_TOKEN is required for local "
            "issue or pull-request API access"
        )

    return token


def repository_token(repository: str) -> str:
    """Mint a repository-restricted GitHub App installation token."""
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


def token_for_source(
    source: dict[str, Any],
    *,
    api_required: bool,
) -> str:
    """Select authentication according to the source access mode."""
    access = source["access"]

    if access == ACCESS_LOCAL:
        if api_required:
            return built_in_github_token()

        return os.environ.get(
            "ARCH_EVIDENCE_GITHUB_TOKEN",
            "",
        ).strip()

    if access == ACCESS_GITHUB_APP:
        return repository_token(source["repository"])

    raise EvidenceError(f"unsupported source access mode {access!r}")


def resolve_api_commit(
    repository: str,
    ref: str,
    token: str,
) -> str:
    """Resolve a GitHub ref to a full commit SHA using the REST API."""
    owner, name = split_repository(repository)

    result = api_request(
        "GET",
        (
            f"/repos/{quote_path_component(owner)}/"
            f"{quote_path_component(name)}/commits/"
            f"{quote_path_component(ref)}"
        ),
        token=token,
    )

    sha = result.get("sha")

    if not isinstance(sha, str) or not FULL_SHA_PATTERN.fullmatch(sha):
        raise EvidenceError(
            f"GitHub returned an invalid commit SHA for "
            f"{repository}@{ref}"
        )

    return sha


def run_git(
    arguments: Iterable[str],
    *,
    environment: dict[str, str] | None = None,
    text: bool = False,
    allow_failure: bool = False,
) -> bytes | str | None:
    """Run Git without invoking a shell."""
    process_environment = os.environ.copy()

    if environment:
        process_environment.update(environment)

    completed = subprocess.run(
        ["git", *arguments],
        check=False,
        capture_output=True,
        env=process_environment,
        text=text,
    )

    if completed.returncode != 0:
        if allow_failure:
            return None

        stderr = (
            completed.stderr
            if isinstance(completed.stderr, str)
            else completed.stderr.decode("utf-8", errors="replace")
        )

        last_line = stderr.strip().splitlines()[-1:] or ["unknown error"]

        raise EvidenceError(
            f"git {next(iter(arguments), '')} failed: {last_line[0]}"
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


def git_auth_environment(
    token: str,
    helper_directory: Path,
) -> dict[str, str]:
    """Create environment variables for authenticated Git HTTPS access."""
    environment = {
        "GIT_TERMINAL_PROMPT": "0",
    }

    if token:
        askpass = create_askpass(helper_directory)
        environment.update(
            {
                "GIT_ASKPASS": str(askpass),
                "ARCH_REPOSITORY_TOKEN": token,
            }
        )

    return environment


def local_checkout_path() -> Path:
    """Return and validate the separately checked-out local source."""
    configured = os.environ.get(
        "ARCH_LOCAL_SOURCE_PATH",
        ".architecture-local-source",
    )

    path = Path(configured).resolve()

    result = run_git(
        ["-C", str(path), "rev-parse", "--is-inside-work-tree"],
        text=True,
        allow_failure=True,
    )

    if result is None or str(result).strip() != "true":
        raise EvidenceError(
            f"local source checkout is not a Git working tree: {path}"
        )

    return path


def git_commit_exists(checkout: Path, sha: str) -> bool:
    """Return whether a commit exists in a local Git object database."""
    result = run_git(
        [
            "-C",
            str(checkout),
            "cat-file",
            "-e",
            f"{sha}^{{commit}}",
        ],
        allow_failure=True,
    )

    return result is not None


def ensure_local_commit(
    checkout: Path,
    sha: str,
    token: str,
    *,
    fetch_ref: str | None = None,
) -> None:
    """Ensure that an immutable commit is present in the local checkout."""
    if git_commit_exists(checkout, sha):
        return

    with tempfile.TemporaryDirectory(
        prefix="architecture-git-auth-"
    ) as helper_directory:
        environment = git_auth_environment(
            token,
            Path(helper_directory),
        )

        fetch_target = fetch_ref or sha

        run_git(
            [
                "-C",
                str(checkout),
                "fetch",
                "--quiet",
                "--no-tags",
                "--depth=1",
                "origin",
                fetch_target,
            ],
            environment=environment,
        )

        environment["ARCH_REPOSITORY_TOKEN"] = ""

    if not git_commit_exists(checkout, sha):
        raise EvidenceError(
            f"local checkout does not contain expected commit {sha}"
        )


def resolve_local_commit(
    checkout: Path,
    ref: str,
    token: str,
) -> str:
    """Resolve a configured ref using the separate local Git checkout."""
    candidates: list[str] = []

    if FULL_SHA_PATTERN.fullmatch(ref):
        candidates.append(ref)
    else:
        candidates.extend(
            [
                f"refs/remotes/origin/{ref}",
                f"refs/heads/{ref}",
                f"refs/tags/{ref}",
                ref,
            ]
        )

    for candidate in candidates:
        result = run_git(
            [
                "-C",
                str(checkout),
                "rev-parse",
                "--verify",
                f"{candidate}^{{commit}}",
            ],
            text=True,
            allow_failure=True,
        )

        if result is None:
            continue

        sha = str(result).strip()

        if FULL_SHA_PATTERN.fullmatch(sha):
            return sha

    with tempfile.TemporaryDirectory(
        prefix="architecture-git-auth-"
    ) as helper_directory:
        environment = git_auth_environment(
            token,
            Path(helper_directory),
        )

        run_git(
            [
                "-C",
                str(checkout),
                "fetch",
                "--quiet",
                "--no-tags",
                "--depth=1",
                "origin",
                ref,
            ],
            environment=environment,
        )

        result = run_git(
            [
                "-C",
                str(checkout),
                "rev-parse",
                "--verify",
                "FETCH_HEAD^{commit}",
            ],
            environment=environment,
            text=True,
        )

        environment["ARCH_REPOSITORY_TOKEN"] = ""

    sha = str(result).strip()

    if not FULL_SHA_PATTERN.fullmatch(sha):
        raise EvidenceError(
            f"local Git returned an invalid commit for ref {ref!r}"
        )

    return sha


def checkout_remote_repository(
    repository: str,
    sha: str,
    token: str,
    *,
    fetch_ref: str | None = None,
) -> tuple[
    tempfile.TemporaryDirectory[str],
    Path,
    dict[str, str],
]:
    """Fetch one exact remote commit into a temporary checkout."""
    temporary = tempfile.TemporaryDirectory(
        prefix="architecture-source-"
    )
    root = Path(temporary.name)
    checkout = root / "repository"
    checkout.mkdir()

    environment = git_auth_environment(token, root)

    try:
        run_git(
            ["init", "--quiet", str(checkout)],
            environment=environment,
        )

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

        fetch_target = fetch_ref or sha

        run_git(
            [
                "-C",
                str(checkout),
                "fetch",
                "--quiet",
                "--no-tags",
                "--depth=1",
                "origin",
                fetch_target,
            ],
            environment=environment,
        )

        fetched_sha = str(
            run_git(
                [
                    "-C",
                    str(checkout),
                    "rev-parse",
                    "FETCH_HEAD^{commit}",
                ],
                environment=environment,
                text=True,
            )
        ).strip()

        if fetched_sha != sha:
            raise EvidenceError(
                f"fetched {fetched_sha}, expected {sha}"
            )

        return temporary, checkout, environment
    except Exception:
        environment["ARCH_REPOSITORY_TOKEN"] = ""
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
    """Match a repository path against a configured glob."""
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
    """Apply built-in denials and configured path rules."""
    normalized = normalize_git_path(path)

    if any(
        glob_match(pattern, normalized)
        for pattern in DENIED_PATH_PATTERNS
    ):
        return False

    if not any(
        glob_match(pattern, normalized)
        for pattern in include_patterns
    ):
        return False

    if any(
        glob_match(pattern, normalized)
        for pattern in exclude_patterns
    ):
        return False

    return True


def redact(content: bytes) -> bytes:
    """Redact obvious credential formats before model exposure."""
    redacted = content

    for pattern in SECRET_PATTERNS:
        redacted = pattern.sub(
            b"[REDACTED-SECRET]",
            redacted,
        )

    return redacted


def safe_destination(
    root: Path,
    relative_path: str,
) -> Path:
    """Resolve an evidence destination while preventing traversal."""
    resolved_root = root.resolve()
    target = (
        resolved_root
        / Path(*PurePosixPath(relative_path).parts)
    ).resolve()

    try:
        target.relative_to(resolved_root)
    except ValueError as error:
        raise EvidenceError(
            f"unsafe evidence destination for {relative_path!r}"
        ) from error

    return target


def collect_from_checkout(
    source: dict[str, Any],
    sha: str,
    checkout: Path,
    output_root: Path,
    *,
    namespace: str | None = None,
    restrict_paths: set[str] | None = None,
) -> dict[str, Any]:
    """Materialize allowlisted blobs from an existing Git checkout."""
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

    listing = run_git(
        [
            "-C",
            str(checkout),
            "ls-tree",
            "-r",
            "-z",
            "--full-tree",
            sha,
        ]
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
                "could not safely parse a Git tree entry"
            ) from error

        normalized_path = normalize_git_path(path)

        if not selected_path(
            normalized_path,
            include_patterns,
            exclude_patterns,
        ):
            continue

        if (
            restrict_paths is not None
            and normalized_path not in restrict_paths
        ):
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
            ]
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

        target = safe_destination(
            destination,
            normalized_path,
        )
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(redact(content))

        total_bytes += size
        included.append(
            {
                "path": normalized_path,
                "bytes": size,
            }
        )

    return {
        "repository": repository,
        "access": source["access"],
        "configured_ref": source["ref"],
        "role": source["role"],
        "commit": sha,
        "namespace": namespace,
        "files": included,
        "skipped": skipped,
    }


def collect_source(
    source: dict[str, Any],
    sha: str,
    token: str,
    output_root: Path,
    *,
    namespace: str | None = None,
    restrict_paths: set[str] | None = None,
    fetch_ref: str | None = None,
) -> dict[str, Any]:
    """Collect a source using local Git or a temporary remote checkout."""
    if source["access"] == ACCESS_LOCAL:
        checkout = local_checkout_path()

        ensure_local_commit(
            checkout,
            sha,
            token,
            fetch_ref=fetch_ref,
        )

        return collect_from_checkout(
            source,
            sha,
            checkout,
            output_root,
            namespace=namespace,
            restrict_paths=restrict_paths,
        )

    if source["access"] == ACCESS_GITHUB_APP:
        temporary, checkout, environment = checkout_remote_repository(
            source["repository"],
            sha,
            token,
            fetch_ref=fetch_ref,
        )

        try:
            return collect_from_checkout(
                source,
                sha,
                checkout,
                output_root,
                namespace=namespace,
                restrict_paths=restrict_paths,
            )
        finally:
            environment["ARCH_REPOSITORY_TOKEN"] = ""
            token = ""
            temporary.cleanup()

    raise EvidenceError(
        f"unsupported source access mode {source['access']!r}"
    )


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

        batch = api_request(
            "GET",
            path,
            token=token,
        )

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
        json.dumps(
            value,
            indent=2,
            sort_keys=False,
        )
        + "\n",
        encoding="utf-8",
    )


def prepare_output(output: Path) -> None:
    """Replace an output directory safely."""
    if output.exists():
        shutil.rmtree(output)

    output.mkdir(parents=True)


def sanitize_text(value: Any) -> str:
    """Convert an API value to redacted UTF-8 text."""
    text = "" if value is None else str(value)

    return redact(
        text.encode("utf-8")
    ).decode(
        "utf-8",
        errors="replace",
    )


def bootstrap(
    scope_path: Path,
    output: Path,
) -> None:
    """Collect all configured bootstrap sources."""
    scope = load_yaml(scope_path)
    validate_scope_runtime(scope)
    prepare_output(output)

    source_results: list[dict[str, Any]] = []

    for source in scope["sources"]:
        token = token_for_source(
            source,
            api_required=False,
        )

        try:
            if source["access"] == ACCESS_LOCAL:
                checkout = local_checkout_path()
                sha = resolve_local_commit(
                    checkout,
                    source["ref"],
                    token,
                )
            else:
                sha = resolve_api_commit(
                    source["repository"],
                    source["ref"],
                    token,
                )

            source_results.append(
                collect_source(
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


def update(
    scope_path: Path,
    reference: str,
    output: Path,
) -> None:
    """Collect one issue or pull request and relevant immutable files."""
    match = CHANGE_REFERENCE_PATTERN.fullmatch(reference)

    if not match:
        raise EvidenceError(
            f"invalid change reference {reference!r}; "
            "expected owner/repository#number"
        )

    repository = match.group(1)
    number = int(match.group(2))

    scope = load_yaml(scope_path)
    validate_scope_runtime(scope)
    source = find_source(scope, repository)

    prepare_output(output)

    token = token_for_source(
        source,
        api_required=True,
    )

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
                raise EvidenceError(
                    "invalid pull-request base SHA"
                )

            if (
                not isinstance(head_sha, str)
                or not FULL_SHA_PATTERN.fullmatch(head_sha)
            ):
                raise EvidenceError(
                    "invalid pull-request head SHA"
                )

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
                collect_source(
                    source,
                    base_sha,
                    token,
                    output,
                    namespace="base",
                    restrict_paths=changed_paths,
                )
            )

            source_results.append(
                collect_source(
                    source,
                    head_sha,
                    token,
                    output,
                    namespace="head",
                    restrict_paths=changed_paths,
                    fetch_ref=f"refs/pull/{number}/head",
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


def verify(
    scope_path: Path,
    architecture_path: Path,
    output: Path,
) -> None:
    """Collect evidence for commits declared in architecture.md."""
    prepare_output(output)
    scope = load_yaml(scope_path)
    validate_scope_runtime(scope)

    rows = verification_rows(architecture_path)

    try:
        source_results: list[dict[str, Any]] = []

        for row in rows:
            source = find_source(
                scope,
                row["repository"],
            )

            token = token_for_source(
                source,
                api_required=False,
            )

            try:
                if source["access"] == ACCESS_LOCAL:
                    checkout = local_checkout_path()

                    ensure_local_commit(
                        checkout,
                        row["sha"],
                        token,
                    )

                    resolved = row["sha"]
                else:
                    resolved = resolve_api_commit(
                        row["repository"],
                        row["sha"],
                        token,
                    )

                if resolved != row["sha"]:
                    raise EvidenceError(
                        "commit verification mismatch for "
                        f"{row['repository']}"
                    )

                source_results.append(
                    collect_source(
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

    bootstrap_parser = subparsers.add_parser(
        "bootstrap"
    )
    bootstrap_parser.add_argument(
        "scope",
        type=Path,
    )
    bootstrap_parser.add_argument(
        "output",
        type=Path,
    )

    update_parser = subparsers.add_parser(
        "update"
    )
    update_parser.add_argument(
        "scope",
        type=Path,
    )
    update_parser.add_argument(
        "reference",
    )
    update_parser.add_argument(
        "output",
        type=Path,
    )

    verify_parser = subparsers.add_parser(
        "verify"
    )
    verify_parser.add_argument(
        "scope",
        type=Path,
    )
    verify_parser.add_argument(
        "architecture",
        type=Path,
    )
    verify_parser.add_argument(
        "output",
        type=Path,
    )

    bootstrap_access_parser = subparsers.add_parser(
        "bootstrap-access"
    )
    bootstrap_access_parser.add_argument(
        "scope",
        type=Path,
    )

    update_access_parser = subparsers.add_parser(
        "update-access"
    )
    update_access_parser.add_argument(
        "scope",
        type=Path,
    )
    update_access_parser.add_argument(
        "reference",
    )

    verify_access_parser = subparsers.add_parser(
        "verify-access"
    )
    verify_access_parser.add_argument(
        "scope",
        type=Path,
    )
    verify_access_parser.add_argument(
        "architecture",
        type=Path,
    )

    return parser


def main() -> int:
    """Program entry point."""
    arguments = build_parser().parse_args()

    try:
        if arguments.command == "bootstrap":
            bootstrap(
                arguments.scope,
                arguments.output,
            )
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
        elif arguments.command == "bootstrap-access":
            print(
                bootstrap_access(arguments.scope)
            )
        elif arguments.command == "update-access":
            print(
                update_access(
                    arguments.scope,
                    arguments.reference,
                )
            )
        elif arguments.command == "verify-access":
            print(
                verification_access(
                    arguments.scope,
                    arguments.architecture,
                )
            )
        else:
            raise EvidenceError(
                f"unsupported command {arguments.command!r}"
            )
    except (
        EvidenceError,
        OSError,
        ValueError,
        KeyError,
        json.JSONDecodeError,
        yaml.YAMLError,
    ) as error:
        print(
            f"evidence collection failed: {error}",
            file=sys.stderr,
        )
        return 1

    return 0


if __name__ == "__main__":
    raise SystemExit(main())