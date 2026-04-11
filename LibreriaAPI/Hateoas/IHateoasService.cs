namespace LibreriaAPI.Hateoas;

/// <summary>
/// Contrato para generar los links HATEOAS de un recurso concreto.
/// </summary>
/// <typeparam name="T">Tipo del DTO del recurso.</typeparam>
public interface IHateoasService<T>
{
    /// <summary>
    /// Genera los links HATEOAS para un recurso individual.
    /// </summary>
    /// <param name="resource">Instancia del recurso.</param>
    /// <param name="httpContext">Contexto HTTP actual (para construir URLs absolutas).</param>
    IReadOnlyList<LinkDto> GenerateLinks(T resource, HttpContext httpContext);

    /// <summary>
    /// Genera los links HATEOAS para la colección completa del recurso.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP actual (para construir URLs absolutas).</param>
    IReadOnlyList<LinkDto> GenerateCollectionLinks(HttpContext httpContext);
}
