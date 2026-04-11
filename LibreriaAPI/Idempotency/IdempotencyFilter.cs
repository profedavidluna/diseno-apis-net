using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LibreriaAPI.Idempotency;

/// <summary>
/// Filtro de acción que implementa el control de idempotencia para endpoints POST.
/// Requiere el encabezado 'Idempotency-Key' en cada solicitud.
/// Si la clave ya fue procesada, devuelve la respuesta cacheada sin ejecutar la acción nuevamente.
/// Un SemaphoreSlim por clave garantiza que solicitudes concurrentes con la misma clave
/// no puedan crear recursos duplicados.
/// </summary>
public class IdempotencyFilter : IAsyncActionFilter
{
    /// <summary>Nombre del encabezado HTTP de idempotencia.</summary>
    public const string IdempotencyKeyHeader = "Idempotency-Key";

    private readonly IIdempotencyService _idempotencyService;
    private readonly JsonSerializerOptions _jsonOptions;

    public IdempotencyFilter(IIdempotencyService idempotencyService, IOptions<JsonOptions> jsonOptions)
    {
        _idempotencyService = idempotencyService;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var idempotencyKey)
            || string.IsNullOrWhiteSpace(idempotencyKey))
        {
            context.Result = new BadRequestObjectResult(
                $"El encabezado '{IdempotencyKeyHeader}' es requerido para esta operación.");
            return;
        }

        var key = idempotencyKey.ToString();
        var semaphore = _idempotencyService.GetOrCreateLock(key);

        await semaphore.WaitAsync();
        try
        {
            var cached = _idempotencyService.GetResponse(key);
            if (cached is not null)
            {
                context.HttpContext.Response.Headers["X-Idempotency-Replayed"] = "true";
                context.Result = new ContentResult
                {
                    StatusCode = cached.StatusCode,
                    ContentType = cached.ContentType,
                    Content = cached.Body
                };
                return;
            }

            var executedContext = await next();

            if (executedContext.Result is ObjectResult { Value: not null } objectResult)
            {
                var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                if (statusCode >= 200 && statusCode < 300)
                {
                    var body = JsonSerializer.Serialize(objectResult.Value, _jsonOptions);
                    _idempotencyService.StoreResponse(
                        key,
                        new IdempotencyEntry(statusCode, "application/json", body));
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
}
