using RSSFeedReader.Api.Services;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

// Read the allowed frontend origin(s) from configuration; no wildcard origins (constitution
// Principle I / Technology & Security Requirements).
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5213", "https://localhost:7135" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<ISubscriptionStore, InMemorySubscriptionStore>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(FrontendCorsPolicy);

/// <summary>Request body for <c>POST /api/subscriptions</c>.</summary>
/// <param name="Url">The feed URL to subscribe to.</param>
app.MapPost("/api/subscriptions", (AddSubscriptionRequest request, ISubscriptionStore store) =>
{
    var url = request.Url?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(url))
    {
        return Results.BadRequest(new { error = "Url is required." });
    }

    var subscription = store.Add(url);
    return Results.Created($"/api/subscriptions/{subscription.Id}", subscription);
})
.WithName("AddSubscription")
.WithOpenApi();

// Returns all subscriptions ordered newest-first; always 200 OK, never 404 (FR-004, FR-010).
app.MapGet("/api/subscriptions", (ISubscriptionStore store) => Results.Ok(store.GetAll()))
    .WithName("GetSubscriptions")
    .WithOpenApi();

app.Run();

record AddSubscriptionRequest(string? Url);

/// <summary>
/// Partial <c>Program</c> declaration exposed so the test project's
/// <c>WebApplicationFactory&lt;Program&gt;</c> can bootstrap this app in-process.
/// </summary>
public partial class Program;
