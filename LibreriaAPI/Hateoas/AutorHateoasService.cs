using LibreriaAPI.DTOs;

namespace LibreriaAPI.Hateoas;

/// <summary>
/// Genera los links HATEOAS para el recurso <see cref="AutorDto"/>.
/// 
/// Links de un recurso individual:
///   - self       → GET    /api/autores/{id}
///   - update     → PUT    /api/autores/{id}
///   - delete     → DELETE /api/autores/{id}
///   - collection → GET    /api/autores
/// 
/// Links de la colección:
///   - self   → GET  /api/autores
///   - create → POST /api/autores
/// </summary>
public class AutorHateoasService : HateoasServiceBase, IHateoasService<AutorDto>
{
    private const string BasePath = "/api/autores";

    public IReadOnlyList<LinkDto> GenerateLinks(AutorDto resource, HttpContext httpContext)
    {
        var baseUrl = GetBaseUrl(httpContext);
        var resourceUrl = $"{baseUrl}{BasePath}/{resource.Id}";
        var collectionUrl = $"{baseUrl}{BasePath}";

        return new List<LinkDto>
        {
            new(resourceUrl,   "self",       "GET"),
            new(resourceUrl,   "update",     "PUT"),
            new(resourceUrl,   "delete",     "DELETE"),
            new(collectionUrl, "collection", "GET"),
        };
    }

    public IReadOnlyList<LinkDto> GenerateCollectionLinks(HttpContext httpContext)
    {
        var baseUrl = GetBaseUrl(httpContext);
        var collectionUrl = $"{baseUrl}{BasePath}";

        return new List<LinkDto>
        {
            new(collectionUrl, "self",   "GET"),
            new(collectionUrl, "create", "POST"),
        };
    }
}
