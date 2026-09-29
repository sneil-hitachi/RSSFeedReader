# Data Model: MVP Feed Subscription Management

## Subscription

Represents a single feed the user has added (spec Key Entity: **Subscription**).

| Field     | Type            | Required | Notes |
|-----------|-----------------|----------|-------|
| `Id`      | `Guid`          | Yes      | Server-generated on creation. Uniquely identifies the entry so duplicate URLs (allowed per spec Assumptions) don't collide as UI list keys. |
| `Url`     | `string`        | Yes      | The feed URL text exactly as submitted by the user. Trimmed of leading/trailing whitespace. MUST be non-empty after trimming (FR-003). No format or reachability validation is performed (FR-006). |
| `AddedAt` | `DateTimeOffset`| Yes      | Server-generated timestamp at creation time. Used solely to order the list newest-first (FR-010). |

### Validation Rules

- `Url` MUST NOT be empty or whitespace-only after trimming → reject the submission (FR-003); no
  entry is created and no error entity is persisted.
- No other validation is applied in this phase (FR-006) — any other non-empty string is accepted
  as-is, including malformed URLs or non-feed addresses.

### Relationships

- None. `Subscription` is a standalone entity in this phase; there are no related entities (no
  user, folder, or feed-item relationships until later phases).

### Lifecycle / State Transitions

- **Created**: via a successful add request (FR-002). This is the only transition in this phase.
- No update, remove, or status-change transitions exist yet (explicitly out of scope per spec
  Assumptions — removing/editing subscriptions is deferred).
- **Destroyed implicitly**: the entire collection is cleared when the backend process stops or
  restarts, because storage is in-memory only for the session (FR-007). This is a store-level
  reset, not a per-entity transition.

### Ordering

- The collection is returned newest-first: sorted by `AddedAt` descending (FR-010).
