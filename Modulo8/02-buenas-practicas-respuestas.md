# Módulo 8 · Experiencia de Usuario en Consumo de APIs
## 02 — Buenas Prácticas en Manejo de Respuestas

> **Objetivo del módulo**
> Una respuesta bien diseñada no sólo entrega datos: comunica estado, orienta al cliente sobre
> qué hacer a continuación y facilita el diagnóstico de problemas. En este documento se cubren
> los estándares y patrones más usados en la industria.

---

## 1. Usar los códigos HTTP correctamente

Los códigos de estado HTTP son el primer canal de comunicación entre la API y el cliente.
Usarlos mal obliga al cliente a parsear el cuerpo para saber si la operación fue exitosa.

### Tabla de referencia rápida

| Código | Nombre | Cuándo usarlo |
|--------|--------|---------------|
| `200` | OK | GET/PUT/PATCH exitoso |
| `201` | Created | POST que crea un recurso |
| `204` | No Content | DELETE exitoso, o PUT/PATCH sin cuerpo de respuesta |
| `400` | Bad Request | El cliente envió datos inválidos |
| `401` | Unauthorized | No autenticado (falta o es inválido el token) |
| `403` | Forbidden | Autenticado pero sin permiso para esta acción |
| `404` | Not Found | El recurso no existe |
| `409` | Conflict | Estado contradictorio (ej. ISBN duplicado) |
| `422` | Unprocessable Entity | Datos bien formados pero semánticamente inválidos |
| `429` | Too Many Requests | Rate limiting activado |
| `500` | Internal Server Error | Error inesperado del servidor |
| `503` | Service Unavailable | Mantenimiento o sobrecarga temporal |

### ❌ Anti-patrón: "200 OK para todo"
```json
HTTP/1.1 200 OK

{
  "success": false,
  "error": "Usuario no encontrado"
}
```
El cliente tiene que leer el cuerpo para saber si falló. Esto rompe clientes HTTP estándar,
middlewares de logging y circuit breakers.

### ✅ Buena práctica
```json
HTTP/1.1 404 Not Found

{
  "type": "https://api.mibiblioteca.com/errors/not-found",
  "title": "Recurso no encontrado",
  "status": 404,
  "detail": "No existe un libro con id=42"
}
```

---

## 2. Estructura de respuesta exitosa

### Patrón recomendado para recursos individuales
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 1,
  "title": "Clean Code",
  "author": "Robert Martin",
  "isbn": "9780132350884",
  "publishedAt": "2008-08-01",
  "stockQuantity": 45,
  "createdAt": "2024-01-10T09:00:00Z",
  "updatedAt": "2024-03-15T14:30:00Z"
}
```

### Patrón recomendado para colecciones con paginación
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "data": [
    { "id": 1, "title": "Clean Code", "author": "Robert Martin" },
    { "id": 2, "title": "The Pragmatic Programmer", "author": "Andrew Hunt" }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 87,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

### 💼 Tip empresarial
Elige una estructura y aplícala en **toda** la API. Considera adoptar estándares abiertos como
[JSON:API](https://jsonapi.org/) o [HAL](https://stateless.group/hal_specification.html) para
evitar reinventar la rueda.

---

## 3. Estructura de respuesta de error (RFC 7807)

El [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) define un formato estándar para
errores HTTP. .NET 8 lo implementa nativamente con `ProblemDetails`.

### Campos estándar
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `type` | URI | Identifica el tipo de error (puede ser una URL con documentación) |
| `title` | string | Descripción corta legible por humanos |
| `status` | int | Código HTTP |
| `detail` | string | Explicación específica de esta instancia del error |
| `instance` | URI | Ruta que causó el error |

### Ejemplo: error de validación
```json
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/problem+json

{
  "type": "https://api.mibiblioteca.com/errors/validation",
  "title": "Error de validación",
  "status": 422,
  "detail": "Se encontraron 2 errores de validación.",
  "instance": "/books",
  "errors": {
    "title":  ["El título es obligatorio."],
    "isbn":   ["El ISBN debe tener exactamente 13 dígitos."]
  }
}
```

### Implementación en .NET 8
```csharp
// Program.cs
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] =
            ctx.HttpContext.TraceIdentifier;
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Usa ProblemDetails para errores de validación de modelos
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Type   = "https://api.mibiblioteca.com/errors/validation",
            Title  = "Error de validación",
            Status = StatusCodes.Status422UnprocessableEntity,
            Instance = context.HttpContext.Request.Path
        };
        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});
```

---

## 4. Cabeceras de respuesta importantes

Las cabeceras HTTP son parte de la respuesta y comunican metadatos clave que los clientes y
proxies necesitan.

### Cabeceras esenciales

| Cabecera | Ejemplo | Propósito |
|----------|---------|-----------|
| `Content-Type` | `application/json; charset=utf-8` | Formato del cuerpo |
| `Location` | `/books/42` | URI del recurso recién creado (en 201) |
| `ETag` | `"abc123"` | Versión del recurso (para caché condicional) |
| `Last-Modified` | `Wed, 01 Jan 2025 00:00:00 GMT` | Fecha de última modificación |
| `Cache-Control` | `max-age=300, public` | Política de caché |
| `X-Request-Id` | `a1b2c3d4` | ID de correlación para trazabilidad |
| `Retry-After` | `60` | Segundos antes de reintentar (429/503) |

### Ejemplo: respuesta de creación correcta
```
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: /books/42
X-Request-Id: 7f3a1b29-c12e-4d8f-a891-0c3f5e6d7a89

{
  "id": 42,
  "title": "Clean Architecture",
  "author": "Robert Martin"
}
```

### ✅ En .NET 8
```csharp
app.MapPost("/books", async (CreateBookDto dto, AppDbContext db) =>
{
    var book = new Book { Title = dto.Title, Author = dto.Author };
    db.Books.Add(book);
    await db.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
    // Automáticamente establece: 201 Created + Location header
});
```

---

## 5. Paginación

Las colecciones grandes nunca deben devolverse completas. Hay tres estrategias principales:

### 5.1 Paginación por offset (la más común)
```
GET /books?page=2&pageSize=20
```
**Ventajas:** Simple de implementar y entender.
**Desventajas:** Inconsistente si los datos cambian mientras se pagina (registros duplicados o
ausentes).

### 5.2 Paginación por cursor (recomendada para feeds en tiempo real)
```
GET /books?cursor=eyJpZCI6MjB9&pageSize=20
```
El cursor es un token opaco (generalmente Base64 de la clave del último elemento).
**Ventajas:** Estable aunque los datos cambien.
**Desventajas:** No permite saltar a una página específica.

### 5.3 Paginación por keyset
```
GET /books?afterId=20&pageSize=20
```
Usa el ID del último elemento como ancla.
**Ventajas:** Eficiente en bases de datos (usa índices).
**Desventajas:** Requiere un campo ordenado y único.

### 💼 Tip empresarial
Para APIs internas o admin, offset es suficiente. Para APIs de producto con alto volumen
(redes sociales, logs, notificaciones), usa cursor-based pagination.

### Implementación offset en .NET 8
```csharp
app.MapGet("/books", async (
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    AppDbContext db) =>
{
    if (page < 1 || pageSize is < 1 or > 100)
        return Results.BadRequest("Parámetros de paginación inválidos.");

    var totalItems = await db.Books.CountAsync();
    var books = await db.Books
        .OrderBy(b => b.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(b => new { b.Id, b.Title, b.Author })
        .ToListAsync();

    return Results.Ok(new
    {
        data = books,
        pagination = new
        {
            page,
            pageSize,
            totalItems,
            totalPages   = (int)Math.Ceiling(totalItems / (double)pageSize),
            hasNextPage  = page * pageSize < totalItems,
            hasPreviousPage = page > 1
        }
    });
});
```

---

## 6. Filtrado, ordenamiento y proyección

### Filtrado
```
GET /books?author=Martin&genre=software
```

### Ordenamiento
```
GET /books?sortBy=publishedAt&sortOrder=desc
```

### Proyección de campos (sparse fieldsets)
Permite al cliente solicitar sólo los campos que necesita, reduciendo el payload.
```
GET /books?fields=id,title,author
```

### ✅ Implementación básica en .NET 8
```csharp
app.MapGet("/books", async (
    [FromQuery] string? author,
    [FromQuery] string? sortBy,
    [FromQuery] string? sortOrder,
    AppDbContext db) =>
{
    var query = db.Books.AsQueryable();

    if (!string.IsNullOrEmpty(author))
        query = query.Where(b => b.Author.Contains(author));

    query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
    {
        ("title", "desc")       => query.OrderByDescending(b => b.Title),
        ("title", _)            => query.OrderBy(b => b.Title),
        ("publishedat", "desc") => query.OrderByDescending(b => b.PublishedAt),
        ("publishedat", _)      => query.OrderBy(b => b.PublishedAt),
        _                       => query.OrderBy(b => b.Id)
    };

    return Results.Ok(await query.ToListAsync());
});
```

### 💼 Tip empresarial
Define una **allowlist** de campos ordenables y filtrables para evitar inyección y consultas
costosas. Nunca permitas `sortBy` con valor arbitrario sin validación.

---

## 7. Manejo de nulos y campos opcionales

### ❌ Anti-patrón: mezclar null y ausencia de campo
Algunos campos que son `null` significa "sin valor", otros significa "no aplicable",
y omitirlos significa "no cargado". Esta ambigüedad confunde al cliente.

### ✅ Buenas prácticas

1. **Campos que pueden no tener valor:** devuelve `null` explícitamente.
2. **Campos que no aplican al tipo de recurso:** omítelos del schema.
3. **Campos de relaciones no cargadas:** usa `null` o proporciona un endpoint separado.

```json
{
  "id": 1,
  "title": "Clean Code",
  "subtitle": null,        ← campo opcional sin valor
  "author": "Robert Martin"
}
```

### Configuración en .NET 8
```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Omite propiedades con null en la serialización
    options.SerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

    // Usa camelCase
    options.SerializerOptions.PropertyNamingPolicy =
        JsonNamingPolicy.CamelCase;
});
```

---

## 8. Respuestas parciales y operaciones batch

### PATCH para actualizaciones parciales
```json
PATCH /books/42
Content-Type: application/json

{
  "stockQuantity": 50
}
```
Solo se actualiza `stockQuantity`. Los demás campos no cambian.

### Operaciones batch (bulk)
Para operaciones masivas, proporciona un endpoint dedicado:
```json
POST /books/bulk
Content-Type: application/json

{
  "operations": [
    { "action": "create", "data": { "title": "Book A", "author": "Author A" } },
    { "action": "update", "id": 5, "data": { "stockQuantity": 100 } },
    { "action": "delete", "id": 12 }
  ]
}
```

Respuesta:
```json
HTTP/1.1 207 Multi-Status

{
  "results": [
    { "action": "create", "status": 201, "id": 101 },
    { "action": "update", "status": 200, "id": 5   },
    { "action": "delete", "status": 404, "id": 12, "error": "No encontrado" }
  ]
}
```

### 💼 Tip empresarial
El código `207 Multi-Status` es perfecto para respuestas batch porque permite reportar éxito
parcial. Procesamiento completo exitoso → `200`, todo falla → código de error apropiado.

---

## Resumen

| Práctica | Impacto en DX |
|----------|---------------|
| Códigos HTTP semánticos | El cliente puede reaccionar sin parsear el cuerpo |
| Estructura consistente | Menor código de integración |
| RFC 7807 para errores | Errores auto-descriptivos y accionables |
| Cabeceras correctas | Habilita caché, trazabilidad y retrocompatibilidad |
| Paginación + filtros | Evita timeouts y payloads enormes |
| Manejo explícito de nulos | Elimina ambigüedad en el cliente |
| Batch con 207 | Reduce round-trips en operaciones masivas |
