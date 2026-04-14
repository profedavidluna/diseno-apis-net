using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LibreriaAPI.IntegrationTests;

public class AutoresCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public AutoresCrudTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        TestAuthHelper.AddBearerToken(_client);
    }

    private async Task<int> CreateAutorAsync(string nombre = "TempAutor", string apellido = "Test")
    {
        TestAuthHelper.AddApiKey(_client);
        var dto = new { nombre, apellido, biografia = "Bio de prueba" };
        var response = await _client.PostAsJsonAsync("/api/autores", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task PutAutor_IdExistente_Retorna204()
    {
        var id = await CreateAutorAsync("ToUpdate");
        var updateDto = new { nombre = "Updated", apellido = "Name", biografia = "New Bio" };

        var response = await _client.PutAsJsonAsync($"/api/autores/{id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PutAutor_IdInexistente_Retorna404()
    {
        var updateDto = new { nombre = "Updated", apellido = "Name", biografia = "Bio" };

        var response = await _client.PutAsJsonAsync("/api/autores/9999", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAutor_IdExistente_Retorna204()
    {
        var id = await CreateAutorAsync("ToDelete");
        TestAuthHelper.AddBearerToken(_client);

        var response = await _client.DeleteAsync($"/api/autores/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAutor_IdInexistente_Retorna404()
    {
        var response = await _client.DeleteAsync("/api/autores/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchAutor_IdExistente_Retorna204()
    {
        var id = await CreateAutorAsync("PatchMe");
        TestAuthHelper.AddBearerToken(_client);

        var patchDoc = "[{\"op\":\"replace\",\"path\":\"/nombre\",\"value\":\"Patched\"}]";
        var content = new StringContent(patchDoc, System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync($"/api/autores/{id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PatchAutor_IdInexistente_Retorna404()
    {
        var patchDoc = "[{\"op\":\"replace\",\"path\":\"/nombre\",\"value\":\"Patched\"}]";
        var content = new StringContent(patchDoc, System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync("/api/autores/9999", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchAutor_PatchDocNulo_Retorna400()
    {
        var content = new StringContent("null", System.Text.Encoding.UTF8, "application/json-patch+json");

        var response = await _client.PatchAsync("/api/autores/1", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
