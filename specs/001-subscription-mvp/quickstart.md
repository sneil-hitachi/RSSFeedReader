# Quickstart: MVP Feed Subscription Management

This guide validates the MVP end-to-end: adding a feed subscription by URL and seeing it appear
in the subscription list, newest-first, with no persistence across restarts.

## Prerequisites

- .NET 8 SDK installed
- Repository cloned and this feature branch (`001-subscription-mvp`) checked out
- `backend/RSSFeedReader.Api` and `frontend/RSSFeedReader.UI` projects created per
  [plan.md](./plan.md) Project Structure and implemented per [tasks.md](./tasks.md)

## 1. Verify template cleanup (constitution Principle III)

Before running the app, confirm the default Blazor demo pages have been removed:

```powershell
Get-ChildItem frontend/RSSFeedReader.UI/Pages/ -Filter *.razor | Select-Object Name
```

Expected: only MVP pages are listed (e.g., `Subscriptions.razor`, `NotFound.razor`) — no
`Home.razor`, `Counter.razor`, or `Weather.razor`.

## 2. Start the backend

```powershell
dotnet run --project backend/RSSFeedReader.Api
```

Expected: the API starts and listens on the port configured in
`backend/RSSFeedReader.Api/Properties/launchSettings.json` (default `http://localhost:5151`) with
no errors.

## 3. Start the frontend

In a separate terminal:

```powershell
dotnet run --project frontend/RSSFeedReader.UI
```

Expected: the Blazor WebAssembly app starts and is reachable in the browser on the port configured
in `frontend/RSSFeedReader.UI/Properties/launchSettings.json` (default `http://localhost:5213`),
with no console errors and no ambiguous-route exceptions.

## 4. Validate configuration alignment

- Confirm `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` → `ApiBaseUrl` matches the backend
  port from step 2.
- Confirm the backend CORS policy in `backend/RSSFeedReader.Api/Program.cs` allow-lists the
  frontend origin from step 3 (no wildcard).
- Open browser DevTools → Console/Network tabs; confirm no CORS or connection errors when the page
  loads.

## 5. Validate User Story 1 — add a subscription

1. Navigate to the app's root page in the browser.
2. Enter a feed URL, e.g. `https://devblogs.microsoft.com/dotnet/feed/`, into the input field.
3. Submit it.

**Expected** (see [contracts/subscriptions-api.md](./contracts/subscriptions-api.md) and spec
FR-001–FR-003): the app sends `POST /api/subscriptions`, receives `201 Created`, the input field
clears, and the new subscription appears in the list within a few seconds (SC-001).

## 6. Validate User Story 2 — view the list, newest-first

1. Add a second, different feed URL following step 5 again.

**Expected** (spec FR-004, FR-005, FR-010, and the Clarifications ordering decision): the list
now shows both subscriptions, with the second URL displayed **above** the first (newest-first),
updated without a manual page reload.

2. Refresh the browser tab (or stop/restart the backend process).

**Expected** (spec FR-007): the subscription list is empty again, confirming storage is
in-memory/session-scoped only, not persisted.

## 7. Validate edge cases

- Submit the form with an empty/whitespace-only URL → expect `400 Bad Request` from the API and no
  new entry added to the list (FR-003).
- Submit the same URL twice → expect two separate entries in the list (duplicates allowed per spec
  Assumptions).

## Success criteria mapping

| Quickstart step | Spec success criteria |
|---|---|
| Steps 5–6 | SC-001 (add-to-display under 5s), SC-002 (100% of valid submissions appear) |
| Step 6 repeated 10+ times | SC-003 (≥10 distinct subscriptions correctly listed) |
| Step 5 (no instructions needed) | SC-004 (first-time usability) |
