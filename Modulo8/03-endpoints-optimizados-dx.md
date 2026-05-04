# Módulo 8 · Experiencia de Usuario en Consumo de APIs
## 03 — Diseño de Endpoints Optimizados para DX

> **DX = Developer Experience**
> Un endpoint optimizado para DX es aquel que un desarrollador puede entender, integrar y
> depurar con el mínimo esfuerzo posible. Este documento cubre las decisiones de diseño que más
> impactan en la calidad de vida del desarrollador que consume tu API.

---

## 1. Convenciones de nomenclatura de URIs

### Reglas fundamentales

| Regla | ✅ Correcto | ❌ Incorrecto |
|-------|------------|--------------|
| Sustantivos en plural | `/books` | `/book`, `/getBooks` |
| Minúsculas | `/library-cards` | `/LibraryCards` |
| Guión medio para separar palabras | `/library-cards` | `/library_cards`, `/librarycards` |
| Sin verbo en la URI | `/books/{id}` | `/books/getById/{id}` |
| Relaciones como sub-recursos | `/books/{id}/reviews` | `/getReviewsByBookId` |
| No usar extensiones de archivo | `/books` | `/books.json` |

### Jerarquía de recursos
```
/books                          → colección raíz
/books/{bookId}                 → recurso individual
/books/{bookId}/reviews         → sub-colección relacionada
/books/{bookId}/reviews/{id}    → elemento de sub-colección
```

### 💼 Tip empresarial
La profundidad recomendada es de máximo **2 niveles de anidamiento** (`/resources/{id}/sub-resources`).
Más niveles hacen las URLs frágiles y difíciles de recordar. Si necesitas más profundidad,
considera devolver referencias con `_links` en lugar de anidar más.

---

## 2. Idempotencia y seguridad de métodos HTTP

Comprender estos conceptos es clave para diseñar endpoints predecibles:

| Método | Seguro* | Idempotente** | Uso típico |
|--------|---------|---------------|------------|
| GET | ✅ | ✅ | Leer recursos |
| HEAD | ✅ | ✅ | Verificar existencia/metadatos |
| OPTIONS | ✅ | ✅ | Descubrir métodos permitidos |
| PUT | ❌ | ✅ | Reemplazar recurso completo |
| DELETE | ❌ | ✅ | Eliminar recurso |
| POST | ❌ | ❌ | Crear recurso, acciones |
| PATCH | ❌ | ❌*** | Actualización parcial |

\* **Seguro:** no modifica estado del servidor.
\** **Idempotente:** múltiples llamadas idénticas producen el mismo resultado.
\*** PATCH puede hacerse idempotente con operaciones absolutas (`"set": value`) en vez de relativas (`"increment": value`).

### ✅ Diseño idempotente de DELETE
```csharp
app.MapDelete("/books/{id}", async (int id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null)
        return Results.NotFound(); // No lanzar error si ya fue eliminado

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent(); // 204 - sin cuerpo
});
```
Llamar `DELETE /books/5` cuando el libro ya no existe devuelve `404`, lo que es
técnicamente correcto. Algunos equipos prefieren devolver `204` también en ese caso para
que el cliente siempre lo trate como operación exitosa (ambos enfoques son válidos; lo
importante es ser consistente).

---

## 3. Diseño de endpoints de acción (verbos)

A veces necesitas expresar acciones que no encajan en CRUD.
El patrón recomendado es usar **sub-recursos de acción** con POST:

```
POST /books/{id}/publish        → publicar un libro
POST /books/{id}/archive        → archivar
POST /orders/{id}/cancel        → cancelar orden
POST /users/{id}/reset-password → resetear contraseña
POST /payments/{id}/refund      → reembolso
```

### ✅ Implementación en .NET 8
```csharp
app.MapPost("/books/{id}/archive", async (int id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null)
        return Results.NotFound();

    if (book.IsArchived)
        return Results.Conflict(new { message = "El libro ya está archivado." });

    book.IsArchived = true;
    book.ArchivedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(book);
});
```

### ❌ Anti-patrón: verbos en la URI con GET
```
GET /archiveBook?id=5      ← modifica estado con GET
GET /sendEmail?userId=10   ← efecto secundario en lectura
```

---

## 4. Búsqueda y filtrado avanzado

### Diseño de endpoint de búsqueda
Para búsquedas complejas, expón un endpoint dedicado:

```
GET /books/search?q=clean+code&author=Martin&genre=software&publishedAfter=2000-01-01
```

O, para queries muy complejas, usa POST con body (aunque no es idempotente):
```
POST /books/search
{
  "query": "clean code",
  "filters": {
    "author": "Martin",
    "genre": ["software", "architecture"],
    "publishedAfter": "2000-01-01",
    "minStock": 1
  },
  "sort": [
    { "field": "publishedAt", "direction": "desc" }
  ],
  "page": 1,
  "pageSize": 20
}
```

### 💼 Tip empresarial
Documentar qué campos son filtrables/ordenables evita que los consumidores prueben
combinaciones que causan full table scans. En APIs de alto tráfico, considera limitar
los campos ordenables a los que tienen índices en la base de datos.

### Implementación en .NET 8 con validación de allowlist
```csharp
app.MapGet("/books/search", async (
    [FromQuery] string? q,
    [FromQuery] string? author,
    [FromQuery] string? sortBy,
    [FromQuery] string? sortOrder,
    AppDbContext db) =>
{
    var allowedSortFields = new HashSet<string>
        { "title", "publishedat", "author" };

    if (!string.IsNullOrEmpty(sortBy) &&
        !allowedSortFields.Contains(sortBy.ToLower()))
    {
        return Results.BadRequest(new
        {
            error = $"Campo de ordenamiento inválido: '{sortBy}'.",
            allowedValues = allowedSortFields
        });
    }

    var query = db.Books.AsQueryable();

    if (!string.IsNullOrEmpty(q))
        query = query.Where(b =>
            b.Title.Contains(q) || b.Author.Contains(q));

    if (!string.IsNullOrEmpty(author))
        query = query.Where(b => b.Author.Contains(author));

    return Results.Ok(await query.Take(100).ToListAsync());
});
```

---

## 5. Versionado de endpoints

### Estrategia URI (recomendada para APIs públicas)
```
/v1/books
/v2/books
```

### Configuración en .NET 8 con Asp.Versioning
```csharp
// Instalar: dotnet add package Asp.Versioning.Http

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Cabecera: api-supported-versions
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),          // /v1/books
        new HeaderApiVersionReader("X-API-Version") // Header alternativo
    );
});

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(1, 0)
    .HasApiVersion(2, 0)
    .ReportApiVersions()
    .Build();

// v1: respuesta simple
app.MapGet("/v{version:apiVersion}/books", async (AppDbContext db) =>
    await db.Books.Select(b => new { b.Id, b.Title }).ToListAsync())
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(1, 0);

// v2: respuesta enriquecida con paginación
app.MapGet("/v{version:apiVersion}/books", async (
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    AppDbContext db) =>
{
    var total = await db.Books.CountAsync();
    var books = await db.Books
        .Skip((page - 1) * pageSize).Take(pageSize)
        .ToListAsync();
    return Results.Ok(new { data = books, pagination = new { page, pageSize, total } });
})
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(2, 0);
```

### Política de deprecación
```csharp
// Marcar v1 como deprecada
options.ApiVersionReader = ...; // configuración anterior

// Cabecera automática cuando se accede a versión deprecada:
// Deprecation: true
// Sunset: <fecha>
```

---

## 6. Rate Limiting

Proteger la API con límites de tasa mejora la estabilidad y la equidad entre consumidores.

### Configuración en .NET 8 (nativo desde .NET 7)
```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    // Límite global: 100 requests por minuto por IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit        = 100,
                Window             = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit         = 0
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers.RetryAfter = "60";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            type   = "https://api.mibiblioteca.com/errors/rate-limit",
            title  = "Demasiadas solicitudes",
            status = 429,
            detail = "Has excedido el límite de 100 solicitudes por minuto.",
            retryAfterSeconds = 60
        }, token);
    };
});

app.UseRateLimiter();
```

### 💼 Tip empresarial
Comunica los límites en cabeceras para que los clientes puedan adaptarse:
```
X-RateLimit-Limit:     100
X-RateLimit-Remaining: 43
X-RateLimit-Reset:     1717200000
```

---

## 7. Caché HTTP

El caché reduce latencia, carga del servidor y costos de infraestructura.

### Tipos de caché

| Tipo | Descripción |
|------|-------------|
| `Cache-Control: public, max-age=300` | Cacheable por proxies y clientes, 5 minutos |
| `Cache-Control: private, max-age=60` | Solo el cliente puede cachear |
| `Cache-Control: no-store` | No cachear (datos sensibles) |
| `ETag` | Hash del recurso para validación condicional |
| `Last-Modified` | Fecha de modificación para validación condicional |

### Caché condicional con ETag en .NET 8
```csharp
app.MapGet("/books/{id}", async (int id, HttpContext http, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    // Generar ETag basado en el contenido
    var etag = $"\"{book.UpdatedAt.Ticks}\"";

    // Si el cliente ya tiene esta versión, no enviar el cuerpo
    if (http.Request.Headers.IfNoneMatch == etag)
        return Results.StatusCode(304); // Not Modified

    http.Response.Headers.ETag = etag;
    http.Response.Headers.CacheControl = "public, max-age=60";

    return Results.Ok(book);
});
```

### 💼 Tip empresarial
Para datos de catálogo (libros, categorías, configuraciones) que cambian poco, `max-age=300`
(5 minutos) puede reducir la carga del servidor en un 80%. Para datos en tiempo real
(stock, precios), usa `no-cache` con ETag para validación condicional sin almacenamiento.

---

## 8. Documentación integrada con OpenAPI / Swagger

Una API bien diseñada está documentada inline. .NET 8 incluye soporte nativo.

```csharp
// Program.cs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Librería API",
        Version     = "v1",
        Description = "API para gestión de biblioteca",
        Contact     = new OpenApiContact
        {
            Name  = "Equipo de desarrollo",
            Email = "api@mibiblioteca.com"
        }
    });
});

// Documentar endpoints con metadatos
app.MapGet("/books/{id}", async (int id, AppDbContext db) => ...)
    .WithName("GetBook")
    .WithSummary("Obtener libro por ID")
    .WithDescription("Devuelve los detalles de un libro específico.")
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithTags("Books");
```

### 💼 Tip empresarial
Considera usar **Scalar** en lugar de Swagger UI para una interfaz más moderna:
```csharp
// dotnet add package Scalar.AspNetCore
app.MapScalarApiReference();
```

---

## 9. Compresión de respuestas

Para payloads grandes (listas, exportaciones), la compresión puede reducir el tamaño hasta
en un 70%.

```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Habilitar en HTTPS
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);

app.UseResponseCompression(); // Antes de los endpoints
```

Los clientes que soporten compresión incluirán `Accept-Encoding: br, gzip` en sus headers
y recibirán la respuesta comprimida automáticamente.

---

## 10. Checklist de DX para cada endpoint

Antes de publicar un endpoint, valida estos puntos:

- [ ] **URI semántica:** usa sustantivo plural, minúsculas, sin verbos.
- [ ] **Método HTTP correcto:** GET/POST/PUT/PATCH/DELETE según la operación.
- [ ] **Código de estado apropiado:** 200/201/204/400/401/403/404/422/429/500.
- [ ] **Respuesta consistente:** misma estructura que otros endpoints.
- [ ] **Errores descriptivos:** formato RFC 7807 con detail útil.
- [ ] **Paginación:** ningún endpoint devuelve colecciones ilimitadas.
- [ ] **Validación de entrada:** todos los campos validados con mensajes claros.
- [ ] **Documentación OpenAPI:** summary, description, response types.
- [ ] **Cabeceras adecuadas:** Content-Type, Location (en 201), ETag (si aplica).
- [ ] **Idempotencia:** PUT/DELETE son idempotentes.
- [ ] **Rate limiting:** el endpoint está bajo control de tasa.
- [ ] **Versión:** el endpoint pertenece a una versión de API.
- [ ] **Seguridad:** autenticación/autorización aplicada.
- [ ] **Prueba con cliente HTTP:** verificado con `.http` file o Postman.

---

## Resumen visual

```
  Cliente
    │
    ├── GET  /v1/books?page=1&sortBy=title    ← URI limpia, query params claros
    │
    ▼
  API Gateway / Load Balancer
    │
    ├── Rate Limit check          ← 429 si excede cuota
    ├── Auth check                ← 401/403 si no autorizado
    ├── Cache check               ← 304 si ETag coincide
    │
    ▼
  Endpoint Handler (.NET 8)
    │
    ├── Validación de input       ← 422 con detalle por campo
    ├── Lógica de negocio
    ├── Paginación                ← nunca colección completa
    │
    ▼
  Respuesta
    ├── 200 OK
    ├── Content-Type: application/json
    ├── Cache-Control: public, max-age=60
    ├── ETag: "abc123"
    ├── X-Request-Id: <trace-id>
    └── Body: { data: [...], pagination: {...} }
```

---

## Recursos adicionales
- [ASP.NET Core Minimal APIs – Microsoft Docs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [Asp.Versioning – NuGet](https://www.nuget.org/packages/Asp.Versioning.Http)
- [Rate Limiting in .NET 7+](https://learn.microsoft.com/aspnet/core/performance/rate-limit)
- [OpenAPI en .NET 9](https://learn.microsoft.com/aspnet/core/fundamentals/openapi/overview)
- [Scalar UI](https://scalar.com/)
- [HTTP Caching – MDN](https://developer.mozilla.org/docs/Web/HTTP/Caching)
