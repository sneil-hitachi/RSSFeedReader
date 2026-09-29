<!--
Sync Impact Report
==================
Version change: [unversioned template] → 1.0.0 (initial ratification)
Modified principles: N/A (first concrete adoption; all placeholders replaced)
Added sections:
  - I. Security by Design (NON-NEGOTIABLE)
  - II. Maintainability Through Incremental Complexity
  - III. Code Quality & Consistency
  - IV. Separation of Concerns
  - V. Testability & Verification
  - Technology & Security Requirements
  - Development Workflow & Quality Gates
  - Governance
Removed sections: None (template placeholders only)
Templates requiring updates:
  - .specify/templates/plan-template.md ⚠ pending manual review for Constitution Check alignment
  - .specify/templates/spec-template.md ⚠ pending manual review
  - .specify/templates/tasks-template.md ⚠ pending manual review
Follow-up TODOs: None — all placeholders resolved from StakeholderDocuments (ProjectGoals.md,
AppFeatures.md, TechStack.md) and today's date.
-->
# RSSFeedReader Constitution

## Core Principles

### I. Security by Design (NON-NEGOTIABLE)
All external inputs (feed URLs, HTTP responses, feed content, user-supplied text) MUST be treated
as untrusted. Validation and/or sanitization MUST be added at the point a phase first consumes
such input in a security-relevant way (e.g., before rendering feed-provided HTML, sanitization via
a library such as HtmlSanitizer is mandatory). CORS policies MUST explicitly allow-list only known
frontend origins from `launchSettings.json`; wildcard (`*`) origins are prohibited. No secrets, API
keys, or connection strings MUST ever be committed to source control; use configuration, user
secrets, or environment variables instead. Rationale: this is a training project, but it must still
model secure coding habits that transfer directly to production systems.

### II. Maintainability Through Incremental Complexity
Features MUST be delivered in the documented phase order — MVP → Extended-MVP → Post-MVP — as
defined in `StakeholderDocuments/ProjectGoals.md`, and MUST NOT be implemented out of that order.
Each phase MUST add only the minimum complexity required to satisfy its own scope; speculative
abstractions or infrastructure (e.g., databases, background services, folders/tags) MUST NOT be
introduced before the phase that explicitly calls for them. Rationale: keeps the codebase small and
understandable for a proof-of-concept while preserving a clear path to production-ready features.

### III. Code Quality & Consistency
Code MUST follow standard C#/.NET naming and style conventions and MUST build with zero warnings
before being merged. Public types/methods that form the contract between frontend and backend
(API models, endpoints) MUST carry XML doc comments or equivalent explanatory documentation.
Default template/demo artifacts (e.g., Blazor's `Home.razor`, `Counter.razor`, `Weather.razor`, and
their nav-menu entries) MUST be removed and routing verified before any MVP feature work begins, to
prevent ambiguous-route failures and dead code accumulating in the repository.

### IV. Separation of Concerns
The backend (ASP.NET Core Web API) MUST own data storage/state and all feed operations (fetching,
parsing); the frontend (Blazor WebAssembly) MUST own presentation and user interaction only.
Business logic MUST NOT leak into UI components. The frontend MUST read the API base URL from
configuration (`wwwroot/appsettings.json`); hardcoding backend URLs in frontend code is prohibited.

### V. Testability & Verification
Any feature beyond MVP subscription management (i.e., feed fetching, parsing, persistence,
background polling) MUST include automated tests (xUnit) covering the new logic before it is
considered complete. Before a phase is marked done, the manual verification steps documented in
`StakeholderDocuments/TechStack.md` (backend/frontend startup, CORS, routing, config alignment)
MUST be performed and confirmed working.

## Technology & Security Requirements

- Stack is fixed as ASP.NET Core Web API (backend) + Blazor WebAssembly (frontend), C# throughout,
  per `StakeholderDocuments/TechStack.md`; changing this requires a constitution amendment.
- MVP storage MUST be in-memory only; no database MUST be introduced until Post-MVP persistence
  work begins (EF Core + SQLite, per TechStack.md).
- Ports and configuration MUST stay synchronized across backend `launchSettings.json`, frontend
  `launchSettings.json`, frontend `appsettings.json` (`ApiBaseUrl`), and the backend CORS policy in
  `Program.cs`; a change to one of these MUST update all four in the same change.
- The Extended-MVP feed parser MUST use `System.ServiceModel.Syndication` unless a documented
  amendment changes this choice.

## Development Workflow & Quality Gates

- Phase 2 (Foundational) MUST include verified removal of default Blazor template pages before any
  UI feature implementation, following the cleanup checklist in `StakeholderDocuments/TechStack.md`.
- Each MVP/Extended-MVP milestone MUST be manually smoke-tested against the acceptance criteria in
  `StakeholderDocuments/ProjectGoals.md` and `StakeholderDocuments/AppFeatures.md` before it is
  marked done.
- Every code change MUST be reviewed against these principles before merge (self-review is
  acceptable for this solo training project, but the review step MUST NOT be skipped).

## Governance

This constitution supersedes ad hoc practices and other stakeholder documents where they conflict;
in a conflict, this constitution's rules govern. Amendments require: a documented rationale, an
update to this file including a Sync Impact Report, and a version bump following semantic
versioning — MAJOR for backward-incompatible governance/principle removals or redefinitions, MINOR
for a new or materially expanded principle/section, PATCH for clarifications or wording fixes. All
pull requests and self-reviews MUST verify compliance with these principles; unjustified complexity
MUST be flagged and simplified before merge.

**Version**: 1.0.0 | **Ratified**: 2026-09-29 | **Last Amended**: 2026-09-29
