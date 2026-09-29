using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

/// <summary>
/// Abstraction over the in-memory subscription store, enabling substitution in tests.
/// </summary>
public interface ISubscriptionStore
{
    /// <summary>
    /// Adds a new subscription for the given URL and returns the created entry.
    /// </summary>
    /// <param name="url">The already-validated, trimmed feed URL.</param>
    Subscription Add(string url);

    /// <summary>
    /// Returns all subscriptions currently stored, ordered newest-first by <see cref="Subscription.AddedAt"/>.
    /// </summary>
    IReadOnlyList<Subscription> GetAll();
}
