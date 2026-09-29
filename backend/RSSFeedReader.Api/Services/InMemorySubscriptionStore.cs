using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

/// <summary>
/// A lock-guarded, in-memory implementation of <see cref="ISubscriptionStore"/>, registered as a
/// DI singleton. Not backed by a database per the MVP's in-memory-only storage constraint.
/// </summary>
public sealed class InMemorySubscriptionStore : ISubscriptionStore
{
    private readonly List<Subscription> _subscriptions = new();
    private readonly object _lock = new();

    /// <inheritdoc />
    public Subscription Add(string url)
    {
        var subscription = new Subscription(Guid.NewGuid(), url, DateTimeOffset.UtcNow);

        lock (_lock)
        {
            _subscriptions.Add(subscription);
        }

        return subscription;
    }

    /// <inheritdoc />
    public IReadOnlyList<Subscription> GetAll()
    {
        lock (_lock)
        {
            return _subscriptions
                .OrderByDescending(s => s.AddedAt)
                .ToList();
        }
    }
}
