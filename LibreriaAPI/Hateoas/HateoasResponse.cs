namespace LibreriaAPI.Hateoas;

/// <summary>
/// Envuelve cualquier dato de respuesta junto con su lista de links HATEOAS.
/// </summary>
/// <typeparam name="T">Tipo del recurso principal (DTO o colección de DTOs).</typeparam>
public class HateoasResponse<T>
{
    /// <summary>Datos principales del recurso.</summary>
    public T Data { get; init; }

    /// <summary>
    /// Links relacionados que el cliente puede seguir para descubrir
    /// las acciones disponibles sobre este recurso.
    /// </summary>
    public IReadOnlyList<LinkDto> Links { get; init; }

    public HateoasResponse(T data, IReadOnlyList<LinkDto> links)
    {
        Data = data;
        Links = links;
    }
}
