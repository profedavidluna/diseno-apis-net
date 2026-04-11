using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LibreriaAPI.Idempotency;

/// <summary>
/// Filtro de operación Swagger que añade automáticamente el parámetro de encabezado
/// 'Idempotency-Key' a todos los endpoints POST.
/// </summary>
public class IdempotencyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.HttpMethod?.Equals("POST", StringComparison.OrdinalIgnoreCase) != true)
            return;

        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IdempotencyFilter.IdempotencyKeyHeader,
            In = ParameterLocation.Header,
            Required = true,
            Description = "Clave única (UUID) para garantizar la idempotencia de la solicitud. " +
                          "Si se repite la misma clave, se devuelve la respuesta original sin crear un recurso duplicado.",
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
        });
    }
}
