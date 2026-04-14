using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace LibreriaAPI.IntegrationTests;

public class CategoriasIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriasIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── GET /api/categorias ─────────────────────────────────────────────────

    [Fact]
    public async Task GetCategorias_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategorias_ConJwt_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── GET /api/categorias/{id} ────────────────────────────────────────────

    [Fact]
    public async Task GetCategoria_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/categorias/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategoria_ConJwtIdExistente_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/categorias/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategoria_ConJwtIdInexistente_Retorna404()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/categorias/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── POST /api/categorias ────────────────────────────────────────────────

    [Fact]
    public async Task PostCategoria_SinApiKey_Retorna401()
    {
        _client.DefaultRequestHeaders.Remove("X-API-Key");
        var dto = new { nombre = "Tecnología", descripcion = "Libros de tecnología" };

        var response = await _client.PostAsJsonAsync("/api/categorias", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostCategoria_ConApiKeyCorrecta_Retorna201()
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new { nombre = "Arte", descripcion = "Libros de arte y diseño" };

        var response = await _client.PostAsJsonAsync("/api/categorias", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
