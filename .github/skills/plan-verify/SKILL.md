---
name: plan-verify
description: Independently verify an implementation plan against the work packet, authoritative architecture documentation, and fixed repository commits.
version: 1.1.0
---

# Plan Verify

Independently verify the planning artifacts.

Do not repair the plan, reinterpret Jira, modify architecture documentation, or
implement the change.

# Handover contract

- Argument: `$1`, the Jira issue key.
- Reads:
  - `.eng/workflows/jira-to-plan.yaml`
  - `run-state.json`
  - the configured work packet
  - the configured architecture context
  - the configured implementation plan
  - the configured `architecture.md`
  - the configured `architecture-map.md`
  - relevant repository files at fixed run-state commits
- Writes: the `verification_report` path resolved from the workflow.
- Must not read:
  - Jira directly
  - conversation history
  - hidden reasoning from earlier skills
  - mutable repository content that differs from the run-state commits
- Returns: output path and verification status only.

# Independence rule

Treat all generated artifacts as claims to verify.

Do not assume they are correct because earlier skills or structural validators
succeeded.

JSON Schema validation proves structural conformance only. This skill performs
semantic verification.

# Architecture authority rules

Use these rules:

1. `architecture.md` is authoritative.
2. `architecture-map.md` is a generated, non-authoritative retrieval index.
3. Map text cannot establish an architecture claim.
4. Stable identifiers selected through the map must resolve in `architecture.md`.
5. Current source code can establish baseline facts only.
6. Current source code cannot establish approved transition or target intent.
7. Only accepted decisions are normative.
8. Proposed decisions require human resolution when implementation depends on them.
9. Rejected, superseded, and obsolete decisions are not active.
10. Architecture evidence-source commits are provenance and not current planning baselines.
11. Run-state commits are the implementation baselines.
12. A missing approved target must not be replaced by an inferred target.

# Phase 0 — Resolve contract

1. Read `.eng/workflows/jira-to-plan.yaml`.
2. Resolve all input and output paths.
3. Read `run-state.json`.
4. Confirm the issue key equals `$1`.
5. Confirm all repository commits are immutable.
6. Confirm all semantic artifacts exist.
7. Confirm all artifacts refer to the same issue and run-state commits.
8. Confirm the output path is inside the write allowlist.
9. Run configured structural validation for:
   - work packet;
   - implementation plan.
10. Stop if identity, commits, paths, or structural validation are inconsistent.

# Phase 1 — Verify work-packet coverage

Check that:

- problem and desired outcome are internally consistent;
- scope is clear;
- confirmed criteria have evidence references;
- draft criteria are not treated as confirmed;
- constraints and dependencies remain visible;
- assumptions, conflicts, and questions are preserved;
- symptoms are not represented as confirmed root causes;
- readiness agrees with unresolved blocking items.

Do not re-read Jira.

If the work packet lacks evidence required to verify a material claim, report the
limitation and require human review.

# Phase 2 — Verify architecture authority

Read the configured architecture files at the fixed workflow-repository commit.

Check that:

- `architecture.md` declares itself authoritative;
- `architecture-map.md` declares itself generated or non-authoritative;
- the map points to the configured authoritative document;
- the plan does not treat the map as an authority;
- selected identifiers exist in `architecture.md`;
- selected claims are supported by the authoritative sections;
- no selected claim exists only in the map;
- the architecture context used the configured files rather than a fallback file.

If authority declarations are missing or contradictory, the result cannot be
`pass`.

# Phase 3 — Verify architecture selection

Check that:

- lifecycle selection is appropriate for the requested work;
- baseline, transition, and target are not conflated;
- baseline observations are supported by documentation or fixed repository content;
- transition claims are explicitly documented;
- target claims are explicitly approved and documented;
- a missing target was not invented;
- mixed components are represented independently;
- migration constraints are preserved;
- temporary transition structures are identified;
- transition owner, exit criteria, and review or removal date are preserved when available;
- approved exceptions are correctly scoped.

For architecture decisions, recognize:

- `proposed`
- `accepted`
- `rejected`
- `superseded`
- `obsolete`

Check that:

- only accepted decisions constrain tasks;
- proposed decisions are not treated as approved;
- rejected decisions are not active;
- superseded decisions are not active;
- obsolete decisions are not active;
- superseding decisions are followed where applicable;
- `deprecated` is not used as an architecture-decision status.

# Phase 4 — Verify provenance and freshness

Read the architecture evidence-source inventory.

Check that the architecture context correctly distinguishes:

- the commit containing `architecture.md`;
- source commits used to generate architecture documentation;
- current run-state implementation commits.

Where repository identities can be matched, compare documented source commits
with run-state commits.

Verify that:

- mismatches are reported as possible freshness gaps;
- mismatches are assessed for materiality;
- unmatched repositories are not silently treated as current;
- historical architecture source commits are not substituted for current
  implementation baselines;
- current source code is not used to rewrite target architecture.

A material unresolved freshness gap requires `needs-human` or `fail`, depending
on whether correction or an authorized decision is required.

# Phase 5 — Verify repository grounding

At the fixed run-state repository commits, inspect every material planned change.

Check that:

- repositories exist;
- projects exist;
- paths exist when the plan says they exist;
- symbols exist when the plan says they exist;
- tests and commands are repository-supported;
- dependency direction matches repository evidence;
- affected interfaces and consumers are credible;
- planned changes do not bypass architecture boundaries;
- no source file was modified by the planning workflow;
- no architecture file was modified by the planning workflow.

Report unsupported path, symbol, project, or command claims as material findings.

# Phase 6 — Verify completeness and traceability

Check that:

- every confirmed acceptance criterion maps to at least one task;
- every confirmed acceptance criterion maps to at least one verification;
- every task maps to acceptance criteria or an explicit technical prerequisite;
- every task has a definition of done;
- task dependencies are coherent and non-circular;
- planned pull requests contain all tasks exactly once unless an explicit
  cross-PR reason exists;
- authoritative architecture constraints map to affected tasks;
- accepted decisions map to affected tasks;
- risks have mitigation or explicit acceptance requirements;
- blocking questions force `needs-clarification`;
- architecture-document changes are delegated to the architecture workflow.

# Phase 7 — Verify quality and delivery

Check that the plan addresses, when applicable:

- positive behavior;
- negative behavior;
- failure behavior;
- authorization;
- auditability;
- privacy and sensitive data;
- transaction boundaries;
- compatibility;
- migration;
- deployment prerequisites;
- rollout;
- abort conditions;
- rollback or forward recovery;
- logs;
- metrics;
- traces;
- alerts;
- health checks;
- post-deployment verification.

Do not require irrelevant ceremony.

Accept `not-applicable` only when the plan gives a credible reason.

# Phase 8 — Determine status

Use `pass` only when:

- no material defect remains;
- the plan is ready for human approval;
- architecture authority is established;
- selected architecture claims resolve in `architecture.md`;
- no proposed decision is treated as normative;
- no material freshness issue remains unresolved;
- repository grounding is credible;
- traceability is complete.

Use `fail` when a correctable material defect exists, including:

- unsupported implementation claims;
- missing traceability;
- invalid lifecycle selection;
- map-only architecture claims;
- use of rejected, superseded, or obsolete decisions as active constraints;
- inferred target architecture;
- unsafe delivery gaps;
- hidden blocking questions;
- misuse of historical architecture source commits as current baselines.

Use `needs-human` when the available artifacts cannot resolve:

- an architecture authority question;
- a proposed architecture decision;
- a missing approved target;
- a material architecture contradiction;
- a material freshness question;
- an ownership or risk-acceptance decision;
- a requirement or evidence question requiring an authorized person.

A `fail` result stops the workflow.

A `needs-human` result requires explicit human approval before publishing or
implementation.

# Phase 9 — Write report

Write Markdown with this YAML frontmatter:

- `artifact_type: verification-report`
- `version: "1.0"`
- `issue_key`: exact Jira key
- `status`: `pass`, `fail`, or `needs-human`
- `generated_at`: ISO-8601 timestamp
- `work_packet`: configured path
- `architecture_context`: configured path
- `implementation_plan`: configured path
- `repository_commits`: comma-separated run-state repository and immutable commit pairs

The body must contain:

1. Verdict.
2. Checks performed.
3. Material findings.
4. Architecture authority findings.
5. Architecture selection findings.
6. Provenance and freshness findings.
7. Repository-grounding findings.
8. Traceability findings.
9. Quality and delivery findings.
10. Required corrections.
11. Human decisions required.
12. Validation limitations.

Each finding must include:

- severity;
- affected artifact or identifier;
- evidence;
- required disposition.

# Phase 10 — Validate

Run the configured validator for artifact type `verification-report`.

Do not modify the input artifacts.

# Completion checks

- Verification was independent from generation.
- `architecture.md` was treated as authoritative.
- `architecture-map.md` was treated only as a retrieval index.
- Stable identifiers were resolved in `architecture.md`.
- Target architecture was not inferred.
- Only accepted decisions were treated as normative.
- Proposed, rejected, superseded, and obsolete decisions were handled correctly.
- Architecture provenance commits were not confused with planning baselines.
- Material freshness gaps were evaluated.
- Material implementation claims were checked at fixed commits.
- Failures were not softened to make the workflow pass.
- Unknowns were not converted into assumptions.
- Only the configured report was written.

# Final response

Return only the output path and verification status.