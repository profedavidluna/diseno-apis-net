namespace LibreriaAPI.Idempotency;

/// <summary>Almacena la respuesta cacheada para una clave de idempotencia.</summary>
public record IdempotencyEntry(int StatusCode, string ContentType, string Body);
