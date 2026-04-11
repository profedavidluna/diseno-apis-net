namespace LibreriaAPI.Hateoas;

/// <summary>
/// Representa un hipervínculo HATEOAS incluido en cada respuesta.
/// </summary>
/// <param name="Href">URL absoluta del recurso o acción.</param>
/// <param name="Rel">Relación semántica del link (self, collection, create, update, delete, …).</param>
/// <param name="Method">Método HTTP que debe usarse para seguir el link.</param>
public record LinkDto(string Href, string Rel, string Method);
