using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

/// <summary>
/// DTO mirroring the backend's <c>Subscription</c> shape returned by
/// <c>contracts/subscriptions-api.md</c>.
/// </summary>
public sealed record SubscriptionDto(Guid Id, string Url, DateTimeOffset AddedAt);

/// <summary>
/// Typed HTTP client wrapper for the subscriptions API, reading its base URL from
/// configuration (<c>ApiBaseUrl</c>) rather than a hardcoded value (constitution Principle IV).
/// </summary>
public sealed class SubscriptionApiClient
{
    private readonly HttpClient _httpClient;

    public SubscriptionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Submits a new subscription URL. Returns the created <see cref="SubscriptionDto"/> on
    /// success, or throws with the API's error message on a 400 response.
    /// </summary>
    public async Task<SubscriptionDto> AddAsync(string url)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/subscriptions", new { url });

        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new InvalidOperationException(problem?.Error ?? "Failed to add subscription.");
        }

        return (await response.Content.ReadFromJsonAsync<SubscriptionDto>())!;
    }

    /// <summary>
    /// Retrieves all subscriptions, ordered newest-first as returned by the API.
    /// </summary>
    public async Task<IReadOnlyList<SubscriptionDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<SubscriptionDto>>("/api/subscriptions")
            ?? new List<SubscriptionDto>();
    }

    private sealed record ErrorResponse(string Error);
}
