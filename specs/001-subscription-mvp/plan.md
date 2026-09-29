# Implementation Plan: MVP Feed Subscription Management

**Branch**: `001-subscription-mvp` | **Date**: 2026-09-29 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-subscription-mvp/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Deliver the MVP subscription-management slice of the RSS feed reader: a user pastes a feed URL
into a Blazor WebAssembly UI, the URL is posted to an ASP.NET Core Web API, stored in an
in-memory, process-lifetime list, and the UI immediately re-displays the full subscription list
newest-first. No feed fetching, parsing, validation, or persistence is included in this phase.

## Technical Context

**Language/Version**: C# / .NET 8 (LTS) — matches the ASP.NET Core Web API + Blazor WebAssembly
stack mandated in `StakeholderDocuments/TechStack.md`; see research.md for version rationale.

**Primary Dependencies**: ASP.NET Core minimal APIs (backend), Blazor WebAssembly (frontend). No
additional NuGet packages are required for this phase (no HTTP client, no feed-parsing library —
those are Extended-MVP concerns per `StakeholderDocuments/TechStack.md`).

**Storage**: In-memory only — a singleton backend service holding subscriptions for the lifetime
of the running process. No database (N/A for this phase per constitution Principle II).

**Testing**: xUnit for backend unit/contract tests (constitution Principle V baseline); manual
smoke test of frontend↔backend flow per the checklist in `StakeholderDocuments/TechStack.md`.

**Target Platform**: Local development — ASP.NET Core Web API host (Windows/macOS/Linux) plus
Blazor WebAssembly running in the developer's browser; both launched via `dotnet run`.

**Project Type**: Web application (frontend + backend) — Option 2 structure below.

**Performance Goals**: Subscription add-and-display round trip completes well under the 5-second
budget in SC-001; as an in-memory, no-network operation this is expected to be near-instant
(sub-200ms) under normal local development conditions.

**Constraints**: In-memory/session-scoped storage only (data lost on restart, per FR-007); no
outbound network calls in this phase (FR-008); CORS MUST allow-list only the configured frontend
origin (constitution Principle I); ports/config MUST stay synchronized across both
`launchSettings.json` files, frontend `appsettings.json`, and backend CORS policy.

**Scale/Scope**: Single local user/session; list MUST correctly hold at least 10 distinct
subscriptions in one session (SC-003). No concurrency, multi-user, or high-volume concerns.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Applies? | Assessment |
|---|---|---|
| I. Security by Design | Yes | CORS MUST allow-list only the configured frontend origin (no wildcard). No secrets/connection strings involved (no external services in this phase). Feed URLs are stored as opaque text and never rendered as HTML or fetched, so no sanitization is required yet — deferred correctly to Extended-MVP per the constitution. **PASS**. |
| II. Maintainability Through Incremental Complexity | Yes | Scope is limited to add + display subscriptions, matching the MVP phase exactly; no database, background service, or feed-fetching code is introduced. **PASS**. |
| III. Code Quality & Consistency | Yes | Plan includes removing default Blazor template pages (`Home.razor`, `Counter.razor`, `Weather.razor`) and their nav entries before feature work, verifying routing, and building with zero warnings. Backend API contract types will carry XML doc comments. **PASS** (actions captured in Project Structure / quickstart). |
| IV. Separation of Concerns | Yes | Backend (Web API) owns the in-memory subscription list and all state; frontend (Blazor WASM) only renders the input form and list, and reads `ApiBaseUrl` from `wwwroot/appsettings.json` rather than hardcoding it. **PASS**. |
| V. Testability & Verification | Yes | Constitution mandates automated tests only for features *beyond* MVP subscription management; this phase still includes baseline xUnit contract tests for the two API endpoints as good practice, plus the required manual smoke test from `StakeholderDocuments/TechStack.md`. **PASS**. |

No violations identified. Complexity Tracking table is not needed for this phase.

**Post-Design re-check (after Phase 1)**: `data-model.md`, `contracts/subscriptions-api.md`, and
`quickstart.md` introduce no new dependencies, storage, or external calls beyond what was assessed
above (a single in-memory entity and two endpoints). All five principles still **PASS** with no
new violations.

## Project Structure

### Documentation (this feature)

```text
specs/001-subscription-mvp/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
backend/RSSFeedReader.Api/
├── Program.cs                       # Minimal API host, endpoint mapping, CORS policy
├── Models/
│   └── Subscription.cs              # Subscription record (Id, Url, AddedAt)
├── Services/
│   ├── ISubscriptionStore.cs        # Storage abstraction (testability)
│   └── InMemorySubscriptionStore.cs # Singleton, in-memory backing store
├── Properties/launchSettings.json
└── appsettings.json

backend/RSSFeedReader.Api.Tests/
└── SubscriptionsEndpointsTests.cs   # xUnit contract tests for the two endpoints

frontend/RSSFeedReader.UI/
├── Pages/
│   └── Subscriptions.razor          # Root ("/") page: add-URL form + subscription list
├── Layout/
│   └── NavMenu.razor                # Updated: demo links removed, points to Subscriptions
├── Services/
│   └── SubscriptionApiClient.cs     # Typed HttpClient wrapper for the backend API
├── Properties/launchSettings.json
└── wwwroot/appsettings.json         # ApiBaseUrl configuration
```

**Structure Decision**: Option 2 (Web application: frontend + backend), matching
`StakeholderDocuments/TechStack.md`. The backend (`backend/RSSFeedReader.Api`) owns state and
exposes the subscription API; the frontend (`frontend/RSSFeedReader.UI`) is a Blazor WebAssembly
app that only renders UI and calls that API. This is a greenfield feature — neither directory
exists yet, so their creation is captured in the Phase 2 tasks (not this plan).
