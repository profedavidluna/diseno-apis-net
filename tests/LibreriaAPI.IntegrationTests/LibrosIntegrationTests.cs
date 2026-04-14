using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace LibreriaAPI.IntegrationTests;

public class LibrosIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LibrosIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── GET /api/libros ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetLibros_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/libros");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLibros_ConJwt_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/libros");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── GET /api/libros/{id} ────────────────────────────────────────────────

    [Fact]
    public async Task GetLibro_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/libros/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLibro_ConJwtIdExistente_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/libros/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLibro_ConJwtIdInexistente_Retorna404()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/libros/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── GET /api/libros/categoria/{categoriaId} ─────────────────────────────

    [Fact]
    public async Task GetLibrosPorCategoria_SinJwt_Retorna401()
    {
        var response = await _client.GetAsync("/api/libros/categoria/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLibrosPorCategoria_ConJwt_Retorna200()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.GetAsync("/api/libros/categoria/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── POST /api/libros ────────────────────────────────────────────────────

    [Fact]
    public async Task PostLibro_SinApiKey_Retorna401()
    {
        _client.DefaultRequestHeaders.Remove("X-API-Key");
        var dto = new
        {
            titulo = "Nuevo Libro",
            descripcion = "Descripción",
            isbn = "978-0-00-000000-0",
            anioPublicacion = 2024,
            categoriaId = 1,
            autoresIds = new[] { 1 }
        };

        var response = await _client.PostAsJsonAsync("/api/libros", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostLibro_ConApiKeyCorrecta_Retorna201()
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new
        {
            titulo = "El Quijote",
            descripcion = "Obra maestra",
            isbn = "978-0-11-111111-1",
            anioPublicacion = 1605,
            categoriaId = 1,
            autoresIds = new[] { 1 }
        };

        var response = await _client.PostAsJsonAsync("/api/libros", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostLibro_ConApiKeyCategoriaInexistente_Retorna404()
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new
        {
            titulo = "Libro Fantasma",
            descripcion = "Descripción",
            isbn = "978-0-22-222222-2",
            anioPublicacion = 2024,
            categoriaId = 9999,
            autoresIds = new[] { 1 }
        };

        var response = await _client.PostAsJsonAsync("/api/libros", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
