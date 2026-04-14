using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace LibreriaAPI.IntegrationTests;

public class AutoresIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AutoresIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── GET /api/autores ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAutores_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/autores");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAutores_ConJwt_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/autores");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── GET /api/autores/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task GetAutor_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/autores/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAutor_ConJwtIdExistente_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/autores/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAutor_ConJwtIdInexistente_Retorna404()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/autores/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── POST /api/autores ───────────────────────────────────────────────────

    [Fact]
    public async Task PostAutor_SinApiKey_Retorna401()
    {
        _client.DefaultRequestHeaders.Remove("X-API-Key");
        var dto = new { nombre = "Juan", apellido = "Pérez", biografia = "Escritor" };

        var response = await _client.PostAsJsonAsync("/api/autores", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostAutor_ConApiKeyCorrecta_Retorna201()
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new { nombre = "Ana", apellido = "García", biografia = "Novelista" };

        var response = await _client.PostAsJsonAsync("/api/autores", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostAutor_ConApiKeyIncorrecta_Retorna401()
    {
        _client.DefaultRequestHeaders.Remove("X-API-Key");
        _client.DefaultRequestHeaders.Add("X-API-Key", "wrong-key");
        var dto = new { nombre = "Juan", apellido = "Pérez", biografia = "Escritor" };

        var response = await _client.PostAsJsonAsync("/api/autores", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
