using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RSSFeedReader.Api.Tests;

/// <summary>
/// Contract tests for the subscriptions API endpoints, per
/// <c>specs/001-subscription-mvp/contracts/subscriptions-api.md</c>.
/// </summary>
public class SubscriptionsEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SubscriptionsEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_WithValidUrl_Returns201WithGeneratedIdAndAddedAt()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/subscriptions", new { url = "https://example.com/feed.xml" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.Equal("https://example.com/feed.xml", created.Url);
        Assert.NotEqual(default, created.AddedAt);
    }

    [Fact]
    public async Task Post_WithEmptyOrWhitespaceUrl_Returns400AndAddsNoEntry()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/subscriptions", new { url = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Url is required.", error!.Error);

        var listResponse = await client.GetAsync("/api/subscriptions");
        var list = await listResponse.Content.ReadFromJsonAsync<List<SubscriptionResponse>>();
        Assert.Empty(list!);
    }

    [Fact]
    public async Task Get_WithNoSubscriptions_Returns200WithEmptyArray()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/subscriptions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<SubscriptionResponse>>();
        Assert.Empty(list!);
    }

    [Fact]
    public async Task Get_WithMultipleSubscriptions_ReturnsNewestFirst()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/subscriptions", new { url = "https://example.com/first.xml" });
        await Task.Delay(10);
        await client.PostAsJsonAsync("/api/subscriptions", new { url = "https://example.com/second.xml" });

        var response = await client.GetAsync("/api/subscriptions");
        var list = await response.Content.ReadFromJsonAsync<List<SubscriptionResponse>>();

        Assert.Equal(2, list!.Count);
        Assert.Equal("https://example.com/second.xml", list[0].Url);
        Assert.Equal("https://example.com/first.xml", list[1].Url);
    }

    private sealed record SubscriptionResponse(Guid Id, string Url, DateTimeOffset AddedAt);

    private sealed record ErrorResponse(string Error);
}
