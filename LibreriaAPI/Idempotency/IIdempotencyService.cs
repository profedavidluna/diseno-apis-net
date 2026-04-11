namespace LibreriaAPI.Idempotency;

/// <summary>Servicio para almacenar y recuperar respuestas cacheadas por clave de idempotencia.</summary>
public interface IIdempotencyService
{
    /// <summary>Obtiene la respuesta cacheada para la clave dada, o null si no existe.</summary>
    IdempotencyEntry? GetResponse(string key);

    /// <summary>Almacena la respuesta para la clave dada con TTL configurable. Si ya existe, no sobreescribe.</summary>
    void StoreResponse(string key, IdempotencyEntry entry);

    /// <summary>Devuelve (o crea) el SemaphoreSlim asociado a la clave para garantizar atomicidad entre la lectura y la escritura.</summary>
    SemaphoreSlim GetOrCreateLock(string key);
}
