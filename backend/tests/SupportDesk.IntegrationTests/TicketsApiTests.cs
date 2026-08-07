using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SupportDesk.IntegrationTests;

public sealed class TicketsApiTests : IClassFixture<SupportDeskApiFactory>
{
    private readonly HttpClient _client;

    public TicketsApiTests(SupportDeskApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-User", "integration.user@example.test");
    }

    [Fact]
    public async Task CreateThenGet_ReturnsPersistedTicket()
    {
        var id = await CreateTicketAsync();

        var response = await _client.GetAsync($"/api/tickets/{id}");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id, body.GetProperty("id").GetGuid());
        Assert.Equal("Open", body.GetProperty("status").GetString());
        Assert.Equal("integration.user@example.test", body.GetProperty("createdBy").GetString());
    }

    [Fact]
    public async Task Search_ReturnsPaginationMetadata()
    {
        await CreateTicketAsync();

        var response = await _client.GetAsync("/api/tickets?priority=High&page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("totalItems").GetInt32() >= 1);
        Assert.Equal(1, body.GetProperty("page").GetInt32());
    }

    [Fact]
    public async Task Update_ChangesOnlyEditableFields()
    {
        var id = await CreateTicketAsync();

        var response = await _client.PutAsJsonAsync($"/api/tickets/{id}", new
        {
            title = "Error crítico al acceder al portal",
            description = "El problema continúa después de aplicar las acciones iniciales de soporte.",
            priority = "Critical"
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Critical", body.GetProperty("priority").GetString());
        Assert.Equal("Open", body.GetProperty("status").GetString());
        Assert.Equal(id, body.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task CommentAndStatusFlow_EnforcesBusinessRules()
    {
        var id = await CreateTicketAsync();

        var commentResponse = await _client.PostAsJsonAsync($"/api/tickets/{id}/comments", new { text = "Se inició el análisis del incidente." });
        var statusResponse = await PatchStatusAsync(id, "InProgress");
        var invalidStatusResponse = await PatchStatusAsync(id, "Closed");
        var commentsResponse = await _client.GetAsync($"/api/tickets/{id}/comments");
        var comments = await commentsResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Created, commentResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, invalidStatusResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, commentsResponse.StatusCode);
        Assert.True(comments.GetArrayLength() >= 1);
    }

    [Fact]
    public async Task InvalidAndMissingRequests_ReturnProblemResponses()
    {
        var invalid = await _client.PostAsJsonAsync("/api/tickets", new { title = "abc", description = "breve", priority = "High" });
        var missing = await _client.GetAsync($"/api/tickets/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal("application/problem+json", invalid.Content.Headers.ContentType?.MediaType);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task InvalidPagination_ReturnsProblemWithTraceId()
    {
        var response = await _client.GetAsync("/api/tickets?page=0&pageSize=101");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("traceId").GetString()));
        Assert.True(body.TryGetProperty("errors", out _));
    }

    private async Task<Guid> CreateTicketAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/tickets", new
        {
            title = "Error al acceder al portal",
            description = "El usuario recibe un error al iniciar sesión en el portal corporativo.",
            priority = "High"
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return body.GetProperty("id").GetGuid();
    }

    private Task<HttpResponseMessage> PatchStatusAsync(Guid id, string status)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/tickets/{id}/status")
        {
            Content = JsonContent.Create(new { status })
        };
        return _client.SendAsync(request);
    }
}
