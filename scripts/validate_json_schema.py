#!/usr/bin/env python3
"""Validate a JSON or YAML document against a JSON Schema."""

from __future__ import annotations

import json
import sys
from pathlib import Path
from typing import Any

import yaml
from jsonschema import Draft202012Validator
from jsonschema.exceptions import SchemaError


def load_document(path: Path) -> Any:
    """Load JSON by extension and otherwise load YAML safely."""
    text = path.read_text(encoding="utf-8")

    if path.suffix.lower() == ".json":
        return json.loads(text)

    return yaml.safe_load(text)


def format_error_path(error_path: object) -> str:
    """Format a jsonschema absolute path as a readable JSON-style path."""
    parts = ["$"]

    for item in error_path:
        if isinstance(item, int):
            parts.append(f"[{item}]")
        else:
            escaped = str(item).replace("\\", "\\\\").replace('"', '\\"')
            parts.append(f'["{escaped}"]')

    return "".join(parts)


def main() -> int:
    if len(sys.argv) != 3:
        print(
            "usage: validate_json_schema.py SCHEMA INSTANCE",
            file=sys.stderr,
        )
        return 2

    schema_path = Path(sys.argv[1])
    instance_path = Path(sys.argv[2])

    try:
        schema = load_document(schema_path)
        instance = load_document(instance_path)

        Draft202012Validator.check_schema(schema)
        validator = Draft202012Validator(schema)

        errors = sorted(
            validator.iter_errors(instance),
            key=lambda error: (
                list(error.absolute_path),
                error.message,
            ),
        )
    except (OSError, json.JSONDecodeError, yaml.YAMLError, SchemaError) as error:
        print(f"validation setup failed: {error}", file=sys.stderr)
        return 1

    if not errors:
        print(f"valid: {instance_path}")
        return 0

    print(
        f"schema validation failed for {instance_path}:",
        file=sys.stderr,
    )

    for error in errors:
        location = format_error_path(error.absolute_path)
        print(f"  - {location}: {error.message}", file=sys.stderr)

    return 1


if __name__ == "__main__":
    raise SystemExit(main())