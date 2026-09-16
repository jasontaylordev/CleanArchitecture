---
name: architecture-synthesize
description: Synthesize a human-reviewable architecture record and a claim-free retrieval index.
---

# Architecture synthesis

The authoritative document is `docs/architecture/architecture.md`.

Use stable identifiers in the form:

```text
ARC-<CATEGORY>-NNN
```

Allowed categories are:

```text
DRV CTX CMP INT DATA SEC DEP OPS BASE TRANS TARGET DEC CON UNK
```

Define each identifier exactly once in a Markdown heading. Preserve existing identifiers when updating an existing concept.

Architecture state rules:

- **Baseline:** currently implemented or operating.
- **Transition:** explicitly approved temporary or intermediate state.
- **Target:** explicitly approved future intent.

Never infer transition or target architecture from implementation evidence. When approval evidence is absent, create an unknown instead.

New decisions created by AI must have status `proposed`.

The architecture map is an index. It may contain only:

- identifiers already defined in the authoritative document;
- the exact title of the authoritative entry;
- category;
- retrieval keywords;
- a link to the authoritative heading.

It must not contain explanatory prose or new claims.