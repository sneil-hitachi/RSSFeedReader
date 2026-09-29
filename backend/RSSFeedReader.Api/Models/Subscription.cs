namespace RSSFeedReader.Api.Models;

/// <summary>
/// Represents a single feed subscription entry, identified by a generated <see cref="Id"/>
/// distinct from its <see cref="Url"/> so duplicate URLs (explicitly allowed for the MVP) do not
/// collide as list identities.
/// </summary>
/// <param name="Id">A generated, stable identifier for this subscription.</param>
/// <param name="Url">
/// The subscribed feed URL. MUST NOT be empty or whitespace-only after trimming.
/// </param>
/// <param name="AddedAt">
/// The timestamp the subscription was added. Used to order subscriptions newest-first.
/// </param>
public sealed record Subscription(Guid Id, string Url, DateTimeOffset AddedAt);
