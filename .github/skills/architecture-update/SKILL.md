---
name: architecture-update
description: Assess one implementation issue or pull request and propose an architecture update only when justified.
---

# Architecture update assessment

Compare the referenced change with the existing architecture record.

Choose one outcome:

- `no_documentation_change`: implementation detail with no material
  architecture effect.
- `human_architecture_input_required`: an architectural decision is needed but
  the evidence does not establish approved intent.
- `insufficient_evidence`: the change cannot be assessed from available
  evidence.
- `architecture_update_proposed`: the evidence demonstrates a material change
  to documented baseline architecture or corrects a demonstrable error.

Material effects can include:

- changed component boundaries or responsibilities;
- new or removed cross-repository dependencies;
- interface or contract changes;
- data ownership changes;
- trust-boundary or security changes;
- deployment topology changes;
- operational or failure-mode changes;
- explicit, human-approved transition or target intent.

Do not turn an implementation plan into approved target architecture. Do not
mark a new decision accepted.

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