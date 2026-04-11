namespace LibreriaAPI.Hateoas;

/// <summary>
/// Clase base que provee utilidades comunes para los servicios HATEOAS.
/// </summary>
public abstract class HateoasServiceBase
{
    /// <summary>
    /// Construye la URL base (esquema + host) a partir del contexto HTTP actual.
    /// </summary>
    protected static string GetBaseUrl(HttpContext httpContext)
        => $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
}
