using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LibreriaAPI.IntegrationTests;

public class LibrosCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LibrosCrudTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateLibroAsync()
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new
        {
            titulo = "TestLibro",
            descripcion = "Desc",
            isbn = "978-0-33-333333-3",
            anioPublicacion = 2020,
            categoriaId = 1,
            autoresIds = new[] { 1 }
        };
        var response = await _client.PostAsJsonAsync("/api/libros", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task PutLibro_IdExistente_Retorna204()
    {
        var id = await CreateLibroAsync();
        var updateDto = new
        {
            titulo = "Updated Libro",
            descripcion = "Updated Desc",
            isbn = "978-0-44-444444-4",
            anioPublicacion = 2021,
            categoriaId = 1,
            autoresIds = new[] { 1 }
        };

        var response = await _client.PutAsJsonAsync($"/api/libros/{id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PutLibro_IdInexistente_Retorna404()
    {
        var updateDto = new
        {
            titulo = "Updated Libro",
            descripcion = "Updated",
            isbn = "978-0-55-555555-5",
            anioPublicacion = 2021,
            categoriaId = 1,
            autoresIds = new[] { 1 }
        };

        var response = await _client.PutAsJsonAsync("/api/libros/9999", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutLibro_CategoriaInexistente_Retorna404()
    {
        var id = await CreateLibroAsync();
        var updateDto = new
        {
            titulo = "Updated",
            descripcion = "Desc",
            isbn = "978-0-66-666666-6",
            anioPublicacion = 2021,
            categoriaId = 9999,
            autoresIds = new[] { 1 }
        };

        var response = await _client.PutAsJsonAsync($"/api/libros/{id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutLibro_AutorInexistente_Retorna400()
    {
        var id = await CreateLibroAsync();
        var updateDto = new
        {
            titulo = "Updated",
            descripcion = "Desc",
            isbn = "978-0-77-777777-7",
            anioPublicacion = 2021,
            categoriaId = 1,
            autoresIds = new[] { 9999 }
        };

        var response = await _client.PutAsJsonAsync($"/api/libros/{id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteLibro_IdExistente_Retorna204()
    {
        var id = await CreateLibroAsync();
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.DeleteAsync($"/api/libros/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteLibro_IdInexistente_Retorna404()
    {
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.DeleteAsync("/api/libros/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchLibro_IdExistente_Retorna204()
    {
        var id = await CreateLibroAsync();
        TestAuthHelper.AddBearerToken(_client);

        var patchDoc = "[{\"op\":\"replace\",\"path\":\"/titulo\",\"value\":\"Patched Titulo\"}]";
        var content = new StringContent(patchDoc, System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync($"/api/libros/{id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PatchLibro_IdInexistente_Retorna404()
    {
        TestAuthHelper.AddBearerToken(_client);
        var patchDoc = "[{\"op\":\"replace\",\"path\":\"/titulo\",\"value\":\"Patched\"}]";
        var content = new StringContent(patchDoc, System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync("/api/libros/9999", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchLibro_PatchDocNulo_Retorna400()
    {
        TestAuthHelper.AddBearerToken(_client);
        var content = new StringContent("null", System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync("/api/libros/1", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLibro_AutoresInexistentes_Retorna400()
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new
        {
            titulo = "BadLibro",
            descripcion = "Desc",
            isbn = "978-0-88-888888-8",
            anioPublicacion = 2024,
            categoriaId = 1,
            autoresIds = new[] { 9999 }
        };

        var response = await _client.PostAsJsonAsync("/api/libros", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
