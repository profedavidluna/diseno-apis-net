using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LibreriaAPI.IntegrationTests;

public class CategoriasCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriasCrudTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        TestAuthHelper.AddBearerToken(_client);
    }

    private async Task<int> CreateCategoriaAsync(string nombre = "TempCat")
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new { nombre, descripcion = "Descripción de prueba" };
        var response = await _client.PostAsJsonAsync("/api/categorias", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task PutCategoria_IdExistente_Retorna204()
    {
        var id = await CreateCategoriaAsync("ToUpdate");
        var updateDto = new { nombre = "UpdatedCat", descripcion = "Updated Desc" };

        var response = await _client.PutAsJsonAsync($"/api/categorias/{id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PutCategoria_IdInexistente_Retorna404()
    {
        var updateDto = new { nombre = "UpdatedCat", descripcion = "Updated Desc" };

        var response = await _client.PutAsJsonAsync("/api/categorias/9999", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategoria_IdExistente_Retorna204()
    {
        var id = await CreateCategoriaAsync("ToDelete");
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.DeleteAsync($"/api/categorias/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategoria_IdInexistente_Retorna404()
    {
        var response = await _client.DeleteAsync("/api/categorias/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchCategoria_IdExistente_Retorna204()
    {
        var id = await CreateCategoriaAsync("PatchCat");
        TestAuthHelper.AddBearerToken(_client);

        var patchDoc = "[{\"op\":\"replace\",\"path\":\"/nombre\",\"value\":\"PatchedCat\"}]";
        var content = new StringContent(patchDoc, System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync($"/api/categorias/{id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PatchCategoria_IdInexistente_Retorna404()
    {
        var patchDoc = "[{\"op\":\"replace\",\"path\":\"/nombre\",\"value\":\"PatchedCat\"}]";
        var content = new StringContent(patchDoc, System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync("/api/categorias/9999", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchCategoria_PatchDocNulo_Retorna400()
    {
        var content = new StringContent("null", System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync("/api/categorias/1", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
