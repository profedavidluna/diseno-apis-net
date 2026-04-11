using LibreriaAPI.DTOs;

namespace LibreriaAPI.Hateoas;

/// <summary>
/// Genera los links HATEOAS para el recurso <see cref="CategoriaDto"/>.
/// 
/// Links de un recurso individual:
///   - self       → GET    /api/categorias/{id}
///   - update     → PUT    /api/categorias/{id}
///   - delete     → DELETE /api/categorias/{id}
///   - collection → GET    /api/categorias
///   - libros     → GET    /api/libros/categoria/{id}  (recurso relacionado)
/// 
/// Links de la colección:
///   - self   → GET  /api/categorias
///   - create → POST /api/categorias
/// </summary>
public class CategoriaHateoasService : HateoasServiceBase, IHateoasService<CategoriaDto>
{
    private const string BasePath = "/api/categorias";

    public IReadOnlyList<LinkDto> GenerateLinks(CategoriaDto resource, HttpContext httpContext)
    {
        var baseUrl = GetBaseUrl(httpContext);
        var resourceUrl = $"{baseUrl}{BasePath}/{resource.Id}";
        var collectionUrl = $"{baseUrl}{BasePath}";
        var librosUrl = $"{baseUrl}/api/libros/categoria/{resource.Id}";

        return new List<LinkDto>
        {
            new(resourceUrl,   "self",       "GET"),
            new(resourceUrl,   "update",     "PUT"),
            new(resourceUrl,   "delete",     "DELETE"),
            new(collectionUrl, "collection", "GET"),
            new(librosUrl,     "libros",     "GET"),
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
