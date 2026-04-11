# LibreriaAPI

API REST para una librería construida con **.NET 8**, **Entity Framework Core (InMemory)** y documentación con **Swagger**.

## Entidades

- **Categoría** – Clasifica los libros (Ficción, Ciencia, Historia, etc.)
- **Autor** – Persona que escribió uno o más libros
- **Libro** – Pertenece a una categoría y puede tener varios autores

## Endpoints disponibles

### Categorías `/api/categorias`
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/categorias` | Obtiene todas las categorías |
| GET    | `/api/categorias/{id}` | Obtiene una categoría por ID |
| POST   | `/api/categorias` | Crea una nueva categoría |
| PUT    | `/api/categorias/{id}` | Actualiza una categoría |
| DELETE | `/api/categorias/{id}` | Elimina una categoría |

### Autores `/api/autores`
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/autores` | Obtiene todos los autores |
| GET    | `/api/autores/{id}` | Obtiene un autor por ID |
| POST   | `/api/autores` | Crea un nuevo autor |
| PUT    | `/api/autores/{id}` | Actualiza un autor |
| DELETE | `/api/autores/{id}` | Elimina un autor |

### Libros `/api/libros`
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/libros` | Obtiene todos los libros con categoría y autores |
| GET    | `/api/libros/{id}` | Obtiene un libro por ID |
| GET    | `/api/libros/categoria/{categoriaId}` | Obtiene libros por categoría |
| POST   | `/api/libros` | Crea un nuevo libro |
| PUT    | `/api/libros/{id}` | Actualiza un libro |
| DELETE | `/api/libros/{id}` | Elimina un libro |

## Cómo ejecutar

```bash
cd LibreriaAPI
dotnet run
```

La aplicación inicia en `http://localhost:5091` (o el puerto configurado).  
La documentación Swagger estará disponible en la raíz: `http://localhost:5091/`

## Datos de prueba (seed data)

Al iniciar la aplicación se cargan automáticamente datos de prueba:

**Categorías:** Ficción, Ciencia, Historia

**Autores:** Gabriel García Márquez, Yuval Noah Harari, Isabel Allende

**Libros:**
- *Cien años de soledad* (Ficción) — García Márquez
- *Sapiens: De animales a dioses* (Ciencia) — Harari
- *La casa de los espíritus* (Ficción) — Isabel Allende

## Tecnologías

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8 (InMemory)
- Swashbuckle (Swagger / OpenAPI 3.0)
