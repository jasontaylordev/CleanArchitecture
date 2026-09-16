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