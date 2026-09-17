---
name: architecture-verify
description: Independently challenge an architecture proposal against evidence without approving or editing it.
---

# Independent architecture verification

Assume the proposal may contain mistakes.

For each substantive claim, ask:

1. Is it baseline, transition, or target?
2. What immutable evidence supports it?
3. Does the cited evidence actually establish the claim?
4. Is an interpretation presented as fact?
5. Does another repository contradict it?
6. Has implementation been mistaken for approved intent?
7. Has an AI-created decision been marked accepted?
8. Does the map contain information absent from the authoritative document?

Separate blocking findings from non-blocking questions. Report unavailable
evidence. Never approve or merge the pull request.

## Evidence-path contract

The evidence manifest distinguishes between source provenance and materialized
workspace paths.

For each file entry:

- `path` is the original source-repository path;
- `source_path` is an explicit alias for the original source-repository path;
- `evidence_path` is the path of the filtered materialized copy, relative to
  the directory containing `manifest.json`.

Only `evidence_path` may be used to open source evidence.

For example, given:

```json
{
  "repository": "EdgarAlvarez10/CleanArchitecture",
  "files": [
    {
      "source_path": "src/Web/Program.cs",
      "evidence_path": "EdgarAlvarez10__CleanArchitecture/src/Web/Program.cs"
    }
  ]
}
```

and a manifest at:

```text
.architecture-work/evidence/manifest.json
```

read:

```text
.architecture-work/evidence/EdgarAlvarez10__CleanArchitecture/src/Web/Program.cs
```

Do not read:

```text
.src/Web/Program.cs
```

- If evidence_path is missing or does not resolve to an existing file, record
an evidence-collection problem. 
- Do not search for or guess an alternative unfiltered source path.