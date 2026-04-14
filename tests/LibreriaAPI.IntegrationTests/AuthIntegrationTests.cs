using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LibreriaAPI.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetToken_CredencialesCorrectas_Retorna200ConToken()
    {
        var request = new { usuario = "admin", password = "password123" };

        var response = await _client.PostAsJsonAsync("/api/auth/token", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("token");
    }

    [Fact]
    public async Task GetToken_CredencialesIncorrectas_Retorna401()
    {
        var request = new { usuario = "admin", password = "wrong" };

        var response = await _client.PostAsJsonAsync("/api/auth/token", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetToken_TokenGenerado_EsUsableEnEndpointsProtegidos()
    {
        var request = new { usuario = "admin", password = "password123" };
        var tokenResponse = await _client.PostAsJsonAsync("/api/auth/token", request);
        tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await tokenResponse.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        var token = json.RootElement.GetProperty("token").GetString();
        token.Should().NotBeNullOrEmpty();

        TestAuthHelper.AddBearerToken(_client, token);
        var autoresResponse = await _client.GetAsync("/api/autores");

        autoresResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
