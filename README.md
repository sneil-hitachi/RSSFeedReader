# RSS Feed Reader

A simple RSS/Atom feed reader built to demonstrate core subscription management. This is an MVP
that focuses on the most basic capability — adding and viewing feed subscriptions — without the
complexity of a production-ready reader (no feed fetching, parsing, or content persistence).

## Project structure

```
backend/RSSFeedReader.Api/         ASP.NET Core Web API (minimal API, in-memory storage)
backend/RSSFeedReader.Api.Tests/   xUnit tests for the API
frontend/RSSFeedReader.UI/         Blazor WebAssembly frontend
specs/                             Feature specs, plans, and tasks (spec-driven development)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Getting started

### 1. Run the backend API

```powershell
dotnet run --project backend/RSSFeedReader.Api
```

The API starts at `http://localhost:5151` (see
`backend/RSSFeedReader.Api/Properties/launchSettings.json`) with Swagger UI available at
`/swagger`.

### 2. Run the frontend (in a separate terminal)

```powershell
dotnet run --project frontend/RSSFeedReader.UI
```

The Blazor WebAssembly app starts at `http://localhost:5213` (see
`frontend/RSSFeedReader.UI/Properties/launchSettings.json`) and calls the API using the
`ApiBaseUrl` configured in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`.

### 3. Use the app

Open the frontend URL in a browser, paste a feed URL (e.g.
`https://devblogs.microsoft.com/dotnet/feed/`) into the input field, and submit it. The
subscription appears at the top of the list (newest first). Subscriptions are stored in memory
only and are cleared when the backend restarts.

## Running tests

```powershell
dotnet test backend/RSSFeedReader.Api.Tests
```

## Current features

- Add a feed subscription by URL (`POST /api/subscriptions`)
- View all subscriptions, newest first (`GET /api/subscriptions`)
- In-memory storage (no persistence across restarts)

Feed content is **not** fetched, parsed, or validated in this MVP.

## Documentation

Detailed feature specs, design decisions, and validation steps live under
[`specs/001-subscription-mvp`](specs/001-subscription-mvp/), including:

- [`spec.md`](specs/001-subscription-mvp/spec.md) — feature specification and requirements
- [`plan.md`](specs/001-subscription-mvp/plan.md) — implementation plan
- [`quickstart.md`](specs/001-subscription-mvp/quickstart.md) — end-to-end validation guide
- [`contracts/subscriptions-api.md`](specs/001-subscription-mvp/contracts/subscriptions-api.md) — API contract
