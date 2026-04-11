using LibreriaAPI.DTOs;

namespace LibreriaAPI.Hateoas;

/// <summary>
/// Genera los links HATEOAS para el recurso <see cref="LibroDto"/>.
/// 
/// Links de un recurso individual:
///   - self       → GET    /api/libros/{id}
///   - update     → PUT    /api/libros/{id}
///   - delete     → DELETE /api/libros/{id}
///   - collection → GET    /api/libros
///   - categoria  → GET    /api/categorias/{categoriaId}  (recurso relacionado)
/// 
/// Links de la colección:
///   - self   → GET  /api/libros
///   - create → POST /api/libros
/// </summary>
public class LibroHateoasService : HateoasServiceBase, IHateoasService<LibroDto>
{
    private const string BasePath = "/api/libros";

    public IReadOnlyList<LinkDto> GenerateLinks(LibroDto resource, HttpContext httpContext)
    {
        var baseUrl = GetBaseUrl(httpContext);
        var resourceUrl = $"{baseUrl}{BasePath}/{resource.Id}";
        var collectionUrl = $"{baseUrl}{BasePath}";
        var categoriaUrl = $"{baseUrl}/api/categorias/{resource.CategoriaId}";

        return new List<LinkDto>
        {
            new(resourceUrl,   "self",       "GET"),
            new(resourceUrl,   "update",     "PUT"),
            new(resourceUrl,   "delete",     "DELETE"),
            new(collectionUrl, "collection", "GET"),
            new(categoriaUrl,  "categoria",  "GET"),
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
