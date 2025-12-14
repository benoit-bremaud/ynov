using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

public class EndpointsTests : IClassFixture<ServerHarness>
{
    private readonly ServerHarness _h;
    public EndpointsTests(ServerHarness harness) => _h = harness;

    [Fact]
    public async Task Health_Returns_OK_Text()
    {
        var resp = await _h.Client.GetAsync("health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Equal("text/plain; charset=utf-8", resp.Content.Headers.ContentType!.ToString());
        var body = await resp.Content.ReadAsStringAsync();
        Assert.Equal("OK", body);
    }

    [Fact]
    public async Task Welcome_Get_Returns_JSON()
    {
        var resp = await _h.Client.GetAsync("api/welcome");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType!.ToString());

        var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        Assert.True(doc.RootElement.TryGetProperty("message", out var msg));
        Assert.Contains("Bienvenue", msg.GetString());
    }

    [Fact]
    public async Task Welcome_Post_ValidationError_400()
    {
        var resp = await _h.Client.PostAsJsonAsync("api/welcome", new { /* missing name */ });
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

        // Header must be present
        Assert.True(resp.Headers.TryGetValues("X-Correlation-Id", out var values));
        var cid = Assert.Single(values);
        Assert.False(string.IsNullOrWhiteSpace(cid));

        // JSON must include correlationId
        using var json = await resp.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.True(json!.RootElement.TryGetProperty("error", out _));
        Assert.True(json!.RootElement.TryGetProperty("correlationId", out var cidProp));
        Assert.False(string.IsNullOrWhiteSpace(cidProp.GetString()));
    }

    [Fact]
    public async Task Unknown_Route_Returns_404_Text()
    {
        var resp = await _h.Client.GetAsync("does-not-exist");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        Assert.Equal("text/plain; charset=utf-8", resp.Content.Headers.ContentType!.ToString());
        var body = await resp.Content.ReadAsStringAsync();
        Assert.Equal("Not Found", body);
    }
    [Fact]
    public async Task Welcome_Post_Ok_200()
    {
        var resp = await _h.Client.PostAsJsonAsync("api/welcome", new { name = "Benoît" });
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType!.ToString());

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        Assert.True(doc.RootElement.TryGetProperty("message", out var msg));
        Assert.Contains("Bienvenue sur notre serveur HTTP simple, Beno", msg.GetString()); // tolérant aux accents
    }
    [Fact]
    public async Task Debug_Boom_Returns_500_With_CorrelationId()
    {
        var resp = await _h.Client.GetAsync("debug/boom");
        Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);

        // Header correlation
        Assert.True(resp.Headers.TryGetValues("X-Correlation-Id", out var values));
        var cid = Assert.Single(values);
        Assert.False(string.IsNullOrWhiteSpace(cid));

        // JSON body correlation
        using var json = await resp.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.True(json!.RootElement.TryGetProperty("error", out _));
        Assert.True(json!.RootElement.TryGetProperty("correlationId", out var cidProp));
        Assert.False(string.IsNullOrWhiteSpace(cidProp.GetString()));
    }


}
