---

description: "Task list template for feature implementation"
---

# Tasks: MVP Feed Subscription Management

**Input**: Design documents from `/specs/001-subscription-mvp/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Contract tests are included below because plan.md's Technical Context and Project
Structure explicitly designate `backend/RSSFeedReader.Api.Tests` (xUnit) for the two API
endpoints.

**Organization**: Tasks are grouped by user story to enable independent implementation and
testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2)
- Include exact file paths in descriptions

## Path Conventions

Web app (frontend + backend), per plan.md Project Structure:

- Backend: `backend/RSSFeedReader.Api/`, tests in `backend/RSSFeedReader.Api.Tests/`
- Frontend: `frontend/RSSFeedReader.UI/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create the backend ASP.NET Core Web API project at `backend/RSSFeedReader.Api/` (.NET 8, minimal API template) per plan.md Project Structure
- [X] T002 Create the backend xUnit test project at `backend/RSSFeedReader.Api.Tests/`, referencing `backend/RSSFeedReader.Api`
- [X] T003 Create the frontend Blazor WebAssembly project at `frontend/RSSFeedReader.UI/` (.NET 8) per plan.md Project Structure
- [X] T004 [P] Configure the backend port in `backend/RSSFeedReader.Api/Properties/launchSettings.json` (default `http://localhost:5151`) per StakeholderDocuments/TechStack.md
- [X] T005 [P] Configure the frontend port in `frontend/RSSFeedReader.UI/Properties/launchSettings.json` (default `http://localhost:5213`) per StakeholderDocuments/TechStack.md
- [X] T006 [P] Add the `ApiBaseUrl` setting in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` pointing to the backend port configured in T004

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T007 Remove the default Blazor template demo pages (`Home.razor`, `Counter.razor`, `Weather.razor`) from `frontend/RSSFeedReader.UI/Pages/` and remove their entries from `frontend/RSSFeedReader.UI/Layout/NavMenu.razor` (constitution Principle III — required before any UI feature work)
- [X] T008 Verify no routing conflicts remain: run `dotnet build frontend/RSSFeedReader.UI` and confirm only one page uses `@page "/"` (constitution Principle III / TechStack.md cleanup checklist)
- [X] T009 Create the `Subscription` model in `backend/RSSFeedReader.Api/Models/Subscription.cs` with fields `Id (Guid)`, `Url (string)`, `AddedAt (DateTimeOffset)`; enforce the data-model.md validation rule verbatim: "Url MUST NOT be empty or whitespace-only after trimming"
- [X] T010 [P] Create the `ISubscriptionStore` interface in `backend/RSSFeedReader.Api/Services/ISubscriptionStore.cs` defining `Subscription Add(string url)` and `IReadOnlyList<Subscription> GetAll()` (returned newest-first per data-model.md Ordering)
- [X] T011 Implement `InMemorySubscriptionStore` in `backend/RSSFeedReader.Api/Services/InMemorySubscriptionStore.cs`: a lock-guarded in-memory `List<Subscription>` registered as a DI singleton (research.md decision #3) (depends on T009, T010)
- [X] T012 Wire up `backend/RSSFeedReader.Api/Program.cs`: register `InMemorySubscriptionStore` as the singleton `ISubscriptionStore`, and configure a named CORS policy that allow-lists only the frontend origin from T005 — no wildcard origins (constitution Principle I) (depends on T011)
- [X] T013 [P] Create the `SubscriptionApiClient` service scaffold in `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`: a typed `HttpClient` wrapper reading `ApiBaseUrl` from configuration (T006), with stub methods `AddAsync(string url)` and `GetAllAsync()` (constitution Principle IV — no hardcoded backend URLs)
- [X] T014 Register `SubscriptionApiClient` and its backing `HttpClient` (bound to `ApiBaseUrl`) in `frontend/RSSFeedReader.UI/Program.cs` (depends on T013)

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Add a feed subscription by URL (Priority: P1) 🎯 MVP

**Goal**: A user pastes a feed URL into the app and adds it as a new subscription, which is
visible immediately, without any feed fetching, parsing, or validation beyond non-empty checking.

**Independent Test**: Enter a URL into the input field, submit it, and confirm it appears in the
subscription list per contracts/subscriptions-api.md `POST /api/subscriptions`.

### Tests for User Story 1

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [X] T015 [P] [US1] Contract test in `backend/RSSFeedReader.Api.Tests/SubscriptionsEndpointsTests.cs`: `POST /api/subscriptions` with a valid, non-empty URL returns `201 Created` with a generated `Id`, the submitted `Url`, and an `AddedAt` timestamp (contracts/subscriptions-api.md)
- [X] T016 [P] [US1] Contract test in `backend/RSSFeedReader.Api.Tests/SubscriptionsEndpointsTests.cs`: `POST /api/subscriptions` with an empty or whitespace-only `url` returns `400 Bad Request` with body `{ "error": "Url is required." }` and adds no entry (FR-003)

### Implementation for User Story 1

- [X] T017 [US1] Implement the `POST /api/subscriptions` minimal API endpoint in `backend/RSSFeedReader.Api/Program.cs`: trim the submitted `Url`, reject empty/whitespace-only values with `400` per T016, otherwise call `ISubscriptionStore.Add` and return `201 Created` with the new `Subscription` (FR-002, FR-003, FR-006, FR-008) (depends on T012)
- [X] T018 [US1] Implement `AddAsync` in `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`: POST the trimmed URL as JSON per contracts/subscriptions-api.md and return the deserialized `Subscription`, or surface the `400` error message (depends on T013)
- [X] T019 [US1] Build the add-subscription form in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`: a text input and submit action calling `SubscriptionApiClient.AddAsync` (FR-001) (depends on T018)
- [X] T020 [US1] On successful add, clear the input field (Acceptance Scenario 3) and prepend the returned subscription to the page's in-memory list so it is visible without a manual refresh (FR-005) in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` (depends on T019)

**Checkpoint**: At this point, User Story 1 should be fully functional and testable
independently — adding a URL makes it appear in the list immediately.

---

## Phase 4: User Story 2 - View the current subscription list (Priority: P1)

**Goal**: A user sees all subscriptions added so far, newest-first, with an empty list shown when
none have been added yet.

**Independent Test**: Load the app with no subscriptions and confirm an empty list; add several
subscriptions and confirm all are visible, newest-first, per contracts/subscriptions-api.md
`GET /api/subscriptions`.

### Tests for User Story 2

- [X] T021 [P] [US2] Contract test in `backend/RSSFeedReader.Api.Tests/SubscriptionsEndpointsTests.cs`: `GET /api/subscriptions` returns `200 OK` with `[]` when no subscriptions exist (FR-004, Acceptance Scenario 1 — never `404`)
- [X] T022 [P] [US2] Contract test in `backend/RSSFeedReader.Api.Tests/SubscriptionsEndpointsTests.cs`: `GET /api/subscriptions` returns all added subscriptions ordered by `AddedAt` descending (newest first), per FR-010 and the Clarifications ordering decision

### Implementation for User Story 2

- [X] T023 [US2] Implement the `GET /api/subscriptions` minimal API endpoint in `backend/RSSFeedReader.Api/Program.cs`: return `ISubscriptionStore.GetAll()` ordered newest-first, always `200 OK` (FR-004, FR-010) (depends on T012)
- [X] T024 [US2] Implement `GetAllAsync` in `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`: GET and deserialize the subscription array per contracts/subscriptions-api.md (depends on T013)
- [X] T025 [US2] Load the subscription list on page init in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` via `OnInitializedAsync` calling `GetAllAsync`, rendering an empty-state message when the list is empty (Acceptance Scenario 1) (depends on T024)
- [X] T026 [US2] Render the subscription list in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` using `@key="subscription.Id"` per data-model.md, confirming newest-first order matches what the API returns (FR-010) and that items added via User Story 1 (T020) and items loaded via T025 share the same ordered list (depends on T025, T020)

**Checkpoint**: At this point, User Stories 1 AND 2 both work independently — the full MVP
(add + view, newest-first, in-memory only) is functional end-to-end.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect both user stories

- [X] T027 [P] Add XML doc comments to the `Subscription` model and the two endpoint handlers in `backend/RSSFeedReader.Api` (constitution Principle III)
- [X] T028 [P] Run `dotnet build` on both `backend/RSSFeedReader.Api` and `frontend/RSSFeedReader.UI` and resolve all warnings until the build is clean (constitution Principle III)
- [X] T029 Execute the full quickstart.md manual validation (steps 1-7) and confirm success criteria SC-001–SC-004 all pass
- [X] T030 [P] Review `backend/RSSFeedReader.Api/Program.cs` CORS policy against both `launchSettings.json` files and `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` for port/origin consistency (constitution Technology & Security Requirements)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational (Phase 2) completion
- **User Story 2 (Phase 4)**: Depends on Foundational (Phase 2) completion; T026 also depends on T020 (US1) so both stories' list-rendering logic share one component, but the story's own contract tests and endpoint (T021-T023) have no dependency on US1
- **Polish (Phase 5)**: Depends on both user stories being complete

### Within Each User Story

- Contract tests (T015-T016, T021-T022) SHOULD be written and FAIL before their corresponding endpoint implementation
- Backend endpoint before frontend API client method before frontend UI wiring

### Parallel Opportunities

- Setup tasks T004, T005, T006 can run in parallel (different files)
- Foundational tasks T010 and T013 can run in parallel (different files, no shared dependency)
- Contract tests T015 and T016 can run in parallel; T021 and T022 can run in parallel
- Polish tasks T027, T028, T030 can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch both contract tests for User Story 1 together:
Task: "Contract test: POST /api/subscriptions with a valid URL returns 201 in backend/RSSFeedReader.Api.Tests/SubscriptionsEndpointsTests.cs"
Task: "Contract test: POST /api/subscriptions with an empty URL returns 400 in backend/RSSFeedReader.Api.Tests/SubscriptionsEndpointsTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently (add a URL, see it appear)
5. Demo if ready — note the list will reset on restart since storage is in-memory only

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Demo (MVP!)
3. Add User Story 2 → Test independently → Demo (full newest-first list, verified empty-state)
4. Complete Polish phase → Run quickstart.md end-to-end

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Both user stories are P1; User Story 1 is still the recommended MVP stopping point per the spec's Independent Test framing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
