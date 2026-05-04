# Módulo 8 · Experiencia de Usuario en Consumo de APIs
## 01 — Principios de UX en Diseño de APIs

> **¿Por qué hablar de UX en APIs?**
> Las APIs no tienen pantallas, pero sí tienen *usuarios*: los desarrolladores que las consumen.
> Una API con buena UX (o **DX – Developer Experience**) reduce la fricción, acelera la integración y
> disminuye errores en producción. Una API difícil de entender genera tickets de soporte, bugs y,
> en última instancia, pérdida de clientes.

---

## 1. Consistencia

### Principio
Todos los endpoints, convenciones de nombres, formatos de fecha, códigos de error y estructuras de
respuesta deben seguir las mismas reglas en toda la API.

### Por qué importa
Un desarrollador que aprende a usar `/books` debe poder deducir cómo funciona `/authors` sin leer
documentación adicional. La coherencia reduce la **carga cognitiva**.

### ✅ Buena práctica
```
GET  /books          → lista de libros
GET  /books/{id}     → un libro
POST /books          → crear libro
PUT  /books/{id}     → reemplazar libro
PATCH /books/{id}    → actualización parcial
DELETE /books/{id}   → eliminar libro
```
Aplicar **exactamente el mismo patrón** a todos los recursos.

### ❌ Anti-patrón
```
GET  /getBooks
POST /createNewBook
GET  /book_detail?id=5
DELETE /removeBook/5
```
Cuatro formas distintas de hacer lo mismo → confusión garantizada.

### 💼 Tip empresarial
Documenta un **API Style Guide** (guía de estilo) como artefacto de equipo antes de escribir una
sola línea de código. Empresas como Stripe, Twilio y GitHub lo publican abiertamente. Herramientas
como [Spectral](https://stoplight.io/open-source/spectral) pueden validar automáticamente que todos
los endpoints cumplan el guía de estilo en CI/CD.

---

## 2. Principio de Mínima Sorpresa

### Principio
La API debe comportarse exactamente como el desarrollador espera, sin efectos secundarios ocultos
ni comportamientos sorpresivos.

### Casos de uso

| Escenario | Comportamiento esperado | Comportamiento sorpresivo ❌ |
|---|---|---|
| `DELETE /books/999` (no existe) | `404 Not Found` | `200 OK` con cuerpo vacío |
| `POST /books` con body inválido | `400 Bad Request` + detalle del error | `500 Internal Server Error` |
| `GET /books?page=0` | Primera página o error claro | Comportamiento indefinido |
| Crear un recurso | `201 Created` + `Location` header | `200 OK` sin indicar dónde está el recurso |

### ✅ Ejemplo en .NET 8
```csharp
// Devolver 404 explícito cuando el recurso no existe
app.MapGet("/books/{id}", async (int id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    return book is null
        ? Results.NotFound(new { error = "Libro no encontrado", bookId = id })
        : Results.Ok(book);
});
```

---

## 3. Claridad sobre brevedad

### Principio
Los nombres de campos y rutas deben ser **descriptivos**, no abreviados. El ancho de banda es
barato; la confusión del desarrollador es cara.

### ❌ Anti-patrón
```json
{
  "nm": "Clean Code",
  "auth": "Robert Martin",
  "dt": "2008-08-01",
  "qty": 120
}
```

### ✅ Buena práctica
```json
{
  "title": "Clean Code",
  "author": "Robert Martin",
  "publishedAt": "2008-08-01",
  "stockQuantity": 120
}
```

### 💼 Tip empresarial
Usa **camelCase** para JSON (estándar de facto en REST moderno) y **snake_case** sólo si la
comunidad de tu industria lo requiere (ej. algunas APIs bancarias). Lo importante es **no mezclar**
estilos en la misma API.

---

## 4. Discoverability (Descubribilidad)

### Principio
El desarrollador debe poder explorar la API sin necesidad de documentación exhaustiva. Esto se logra
con **hipermedia** (HATEOAS), paginación con enlaces y respuestas auto-descriptivas.

### Ejemplo de respuesta con HATEOAS básico
```json
{
  "data": [
    { "id": 1, "title": "Clean Code" },
    { "id": 2, "title": "The Pragmatic Programmer" }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 87,
    "totalPages": 9
  },
  "_links": {
    "self":  { "href": "/books?page=1&pageSize=10" },
    "next":  { "href": "/books?page=2&pageSize=10" },
    "last":  { "href": "/books?page=9&pageSize=10" }
  }
}
```

### 💼 Tip empresarial
Exponer una ruta `GET /` o `GET /api` que liste los recursos disponibles es una forma sencilla de
mejorar la discoverability sin implementar HATEOAS completo.

---

## 5. Feedback inmediato y claro

### Principio
Cada operación debe indicar claramente **qué pasó**, **por qué**, y (si hubo error) **cómo
solucionarlo**.

### Estructura de error recomendada (RFC 7807 – Problem Details)
```json
{
  "type": "https://api.mibiblioteca.com/errors/validation",
  "title": "Error de validación",
  "status": 422,
  "detail": "El campo 'isbn' no tiene el formato correcto.",
  "instance": "/books",
  "errors": {
    "isbn": ["El ISBN debe tener 13 dígitos numéricos."]
  }
}
```

### ✅ Implementación en .NET 8 con Problem Details
```csharp
// En Program.cs
builder.Services.AddProblemDetails();

app.UseExceptionHandler();
app.UseStatusCodePages();
```

---

## 6. Tolerancia a errores del cliente

### Principio
Diseña la API para que sea **permisiva en lo que acepta** y **estricta en lo que produce**.
(Postel's Law / Robustness Principle)

### Casos de uso
- Aceptar fechas en múltiples formatos (`2024-01-15`, `15/01/2024`) y normalizarlas internamente.
- Ignorar campos desconocidos en el request en vez de rechazar con `400`.
- Tratar `"true"`, `"1"` y `true` como equivalentes para campos booleanos.

### Advertencia
No llevar este principio al extremo: demasiada tolerancia puede ocultar errores del cliente y
dificultar la evolución de la API.

---

## 7. Versionado desde el principio

### Principio
Planifica la evolución de tu API **desde el día uno**. Los cambios son inevitables; el impacto en
los consumidores no tiene que serlo.

### Estrategias comunes

| Estrategia | Ejemplo | Pros | Contras |
|---|---|---|---|
| URI versioning | `/v1/books` | Fácil de entender y cachear | Contamina las URIs |
| Header versioning | `API-Version: 2` | URIs limpias | Menos visible |
| Query param | `/books?version=2` | Fácil de probar en browser | No es RESTful puro |
| Content negotiation | `Accept: application/vnd.api+json;version=2` | Estándar HTTP | Complejo de implementar |

### 💼 Tip empresarial
La mayoría de las APIs empresariales usan **URI versioning** por su simplicidad operacional.
Mantén al menos la versión anterior activa durante **6-12 meses** después de publicar una nueva,
con cabeceras de deprecación:
```
Deprecation: true
Sunset: Sat, 01 Jun 2026 00:00:00 GMT
Link: <https://api.mibiblioteca.com/v2/books>; rel="successor-version"
```

---

## Resumen de principios

| # | Principio | Beneficio principal |
|---|---|---|
| 1 | Consistencia | Reduce curva de aprendizaje |
| 2 | Mínima sorpresa | Reduce bugs de integración |
| 3 | Claridad sobre brevedad | Mejora mantenibilidad |
| 4 | Discoverability | Permite exploración autónoma |
| 5 | Feedback claro | Acelera depuración |
| 6 | Tolerancia a errores | Aumenta robustez del cliente |
| 7 | Versionado desde el inicio | Facilita evolución sin ruptura |

---

## Recursos adicionales
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines)
- [Zalando RESTful API Guidelines](https://opensource.zalando.com/restful-api-guidelines/)
- [RFC 7807 – Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc7807)
- [The Design of Everyday APIs – Heroku](https://brandur.org/elegant-apis)
