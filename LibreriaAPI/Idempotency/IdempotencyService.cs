using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace LibreriaAPI.Idempotency;

/// <summary>
/// Implementación en memoria del servicio de idempotencia.
/// Usa <see cref="IMemoryCache"/> para expirar automáticamente las claves después de 24 horas,
/// evitando el crecimiento ilimitado del caché.
/// Un <see cref="SemaphoreSlim"/> por clave garantiza que dos solicitudes simultáneas
/// con la misma clave no puedan ejecutar la acción en paralelo.
/// Registrado como Singleton para persistir las claves durante el ciclo de vida de la aplicación.
/// </summary>
public class IdempotencyService : IIdempotencyService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public IdempotencyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IdempotencyEntry? GetResponse(string key)
    {
        _cache.TryGetValue<IdempotencyEntry>(key, out var entry);
        return entry;
    }

    public void StoreResponse(string key, IdempotencyEntry entry)
    {
        _cache.Set(key, entry, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        });
    }

    public SemaphoreSlim GetOrCreateLock(string key) =>
        _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
}
