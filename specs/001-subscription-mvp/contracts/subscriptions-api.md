# API Contract: Subscriptions

**Base path**: `{ApiBaseUrl}/api/subscriptions` (e.g., `http://localhost:5151/api/subscriptions`)

This is the contract exposed by the backend (`backend/RSSFeedReader.Api`) to the frontend
(`frontend/RSSFeedReader.UI`) for the MVP subscription-management slice. No authentication is
required (single-user, local-only app per spec Assumptions).

## GET /api/subscriptions

Returns all subscriptions added in the current session, newest-first.

**Request**: No parameters, no body.

**Response**: `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "url": "https://devblogs.microsoft.com/dotnet/feed/",
    "addedAt": "2026-09-29T18:04:21.000Z"
  }
]
```

- Array is ordered by `addedAt` descending (newest first) — see data-model.md.
- Returns `[]` (empty array) when no subscriptions have been added yet (FR-004, User Story 2
  Acceptance Scenario 1). This is always a `200 OK`, never a `404`.

## POST /api/subscriptions

Adds a new subscription.

**Request**: `Content-Type: application/json`

```json
{
  "url": "https://devblogs.microsoft.com/dotnet/feed/"
}
```

**Response (success)**: `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "url": "https://devblogs.microsoft.com/dotnet/feed/",
  "addedAt": "2026-09-29T18:04:21.000Z"
}
```

- `Location` header points to the new resource (not otherwise retrievable individually in this
  phase, since there is no `GET /api/subscriptions/{id}` endpoint — the list endpoint is the only
  read path needed for FR-004).

**Response (validation failure)**: `400 Bad Request`

```json
{
  "error": "Url is required."
}
```

- Returned when `url` is missing, empty, or whitespace-only after trimming (FR-003). No entry is
  added to the store.

**Notes**:

- No uniqueness check is performed — submitting the same `url` more than once creates additional,
  independent entries (spec Assumptions: duplicates allowed).
- No validation of URL format or reachability is performed beyond the non-empty check (FR-006).
- This endpoint never fetches, parses, or contacts the submitted URL (FR-008).
