# Phase 0 Research: MVP Feed Subscription Management

## 1. .NET version

- **Decision**: Target .NET 8 (LTS).
- **Rationale**: `StakeholderDocuments/TechStack.md` mandates ASP.NET Core Web API + Blazor
  WebAssembly but does not pin a version. .NET 8 is the current Long-Term Support release with
  first-class Blazor WebAssembly and minimal API support, giving the longest support window for a
  training project that will grow across multiple phases (Extended-MVP, Post-MVP).
- **Alternatives considered**: .NET 9 (Standard Term Support — shorter support window, no
  compelling feature needed for this MVP); .NET 6 (LTS but nearing end of support) — rejected in
  favor of the current LTS.

## 2. API style: Minimal APIs vs. Controllers

- **Decision**: Use ASP.NET Core minimal APIs (`app.MapGet` / `app.MapPost` in `Program.cs`).
- **Rationale**: The constitution's Maintainability principle requires adding only the minimum
  complexity a phase needs. Two endpoints (list subscriptions, add subscription) do not justify
  the additional ceremony of full MVC controllers. Minimal APIs also keep `Program.cs` as the
  single, easy-to-read entry point appropriate for a POC.
- **Alternatives considered**: MVC Controllers with attribute routing — more boilerplate (a
  controller class, DI wiring, routing attributes) for no added benefit at this scale; rejected
  for now but not precluded if the API surface grows substantially in later phases.

## 3. In-memory storage mechanism

- **Decision**: A singleton `ISubscriptionStore` service backed by an in-memory `List<Subscription>`
  guarded by a simple lock, registered via `AddSingleton`.
- **Rationale**: A DI-registered singleton service (rather than a static field) keeps state
  testable (the interface can be swapped/mocked in xUnit tests) and avoids the static-state
  anti-pattern, while still satisfying the "in-memory only, no database" constraint from the
  constitution's Technology & Security Requirements. A basic lock is sufficient because Kestrel's
  default single-process, low-concurrency local dev usage does not require a more sophisticated
  concurrent collection for this MVP.
- **Alternatives considered**: `ConcurrentBag<T>`/`ConcurrentQueue<T>` — unnecessary complexity for
  a single local user with no meaningful concurrent writers; static in-memory list — rejected due
  to poor testability and hidden global state.

## 4. Subscription identity & ordering

- **Decision**: Model each subscription as `{ Id: Guid, Url: string, AddedAt: DateTimeOffset }`.
  New entries are appended to the store and the list endpoint returns them ordered by `AddedAt`
  descending (newest first), satisfying FR-010 / the clarified ordering decision.
- **Rationale**: A generated `Id` gives each entry a stable identity distinct from its URL, which
  keeps duplicate URLs (explicitly allowed per the spec's Assumptions) from colliding as list keys
  in the Blazor UI (`@key="subscription.Id"`). `AddedAt` is the simplest reliable basis for
  newest-first ordering without needing a separate sequence counter.
- **Alternatives considered**: Using the raw URL string as the sole identifier — rejected because
  duplicate URLs are allowed and would break UI list-item keys and any future removal feature;
  maintaining a separate insertion-order counter — unnecessary given `AddedAt` already provides a
  sortable value.

## 5. CORS configuration approach

- **Decision**: Read the allowed frontend origin(s) from configuration (`appsettings.json` /
  environment) and register an explicit named CORS policy allow-listing only those origins; no
  wildcard origins.
- **Rationale**: Directly required by constitution Principle I (Security by Design) and the
  Technology & Security Requirements section, which mandate explicit origin allow-listing and keep
  it synchronized with the frontend's `launchSettings.json` ports.
- **Alternatives considered**: `AllowAnyOrigin()` — explicitly prohibited by the constitution;
  rejected.

## Outstanding NEEDS CLARIFICATION

None. All Technical Context unknowns are resolved above.
