using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LibrosController : ControllerBase
{
    private readonly LibreriaContext _context;

    public LibrosController(LibreriaContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene todos los libros con su categoría y autores</summary>
    [HttpGet]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
    {
        var libros = await _context.Libros
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .Select(l => new LibroDto(
                l.Id,
                l.Titulo,
                l.Descripcion,
                l.ISBN,
                l.AnioPublicacion,
                l.CategoriaId,
                l.Categoria != null ? l.Categoria.Nombre : null,
                l.LibroAutores.Select(la => new AutorDto(
                    la.Autor!.Id,
                    la.Autor.Nombre,
                    la.Autor.Apellido,
                    la.Autor.Biografia
                ))
            ))
            .ToListAsync();

        return Ok(libros);
    }

    /// <summary>Obtiene un libro por su ID</summary>
    [HttpGet("{id:int}")]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> GetLibro(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        var result = new LibroDto(
            libro.Id,
            libro.Titulo,
            libro.Descripcion,
            libro.ISBN,
            libro.AnioPublicacion,
            libro.CategoriaId,
            libro.Categoria?.Nombre,
            libro.LibroAutores.Select(la => new AutorDto(
                la.Autor!.Id,
                la.Autor.Nombre,
                la.Autor.Apellido,
                la.Autor.Biografia
            ))
        );

        return Ok(result);
    }

    /// <summary>Obtiene los libros de una categoría específica</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibrosPorCategoria(int categoriaId)
    {
        var libros = await _context.Libros
            .Where(l => l.CategoriaId == categoriaId)
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .Select(l => new LibroDto(
                l.Id,
                l.Titulo,
                l.Descripcion,
                l.ISBN,
                l.AnioPublicacion,
                l.CategoriaId,
                l.Categoria != null ? l.Categoria.Nombre : null,
                l.LibroAutores.Select(la => new AutorDto(
                    la.Autor!.Id,
                    la.Autor.Nombre,
                    la.Autor.Apellido,
                    la.Autor.Biografia
                ))
            ))
            .ToListAsync();

        return Ok(libros);
    }

    /// <summary>Crea un nuevo libro</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> PostLibro(CrearLibroDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _context.Autores
            .Where(a => dto.AutoresIds.Contains(a.Id))
            .ToListAsync();

        if (autores.Count != dto.AutoresIds.Count())
            return BadRequest("Uno o más autores no fueron encontrados.");

        var libro = new Libro
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            ISBN = dto.ISBN,
            AnioPublicacion = dto.AnioPublicacion,
            CategoriaId = dto.CategoriaId
        };

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        foreach (var autor in autores)
        {
            _context.LibroAutores.Add(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, await BuildLibroDto(libro.Id));
    }

    /// <summary>Actualiza un libro existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLibro(int id, ActualizarLibroDto dto)
    {
        var libro = await _context.Libros
            .Include(l => l.LibroAutores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _context.Autores
            .Where(a => dto.AutoresIds.Contains(a.Id))
            .ToListAsync();

        if (autores.Count != dto.AutoresIds.Count())
            return BadRequest("Uno o más autores no fueron encontrados.");

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.CategoriaId = dto.CategoriaId;

        _context.LibroAutores.RemoveRange(libro.LibroAutores);
        foreach (var autor in autores)
        {
            _context.LibroAutores.Add(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Actualiza parcialmente un libro usando JSON Patch (RFC 6902)</summary>
    /// <remarks>
    /// Ejemplo de body (application/json-patch+json):
    ///
    ///     [
    ///       { "op": "replace", "path": "/titulo", "value": "Nuevo Título" },
    ///       { "op": "replace", "path": "/anioPublicacion", "value": 2024 }
    ///     ]
    /// </remarks>
    [HttpPatch("{id:int}")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchLibro(int id, JsonPatchDocument<PatchLibroDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest();

        var libro = await _context.Libros
            .Include(l => l.LibroAutores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        var dto = new PatchLibroDto
        {
            Titulo = libro.Titulo,
            Descripcion = libro.Descripcion,
            ISBN = libro.ISBN,
            AnioPublicacion = libro.AnioPublicacion,
            CategoriaId = libro.CategoriaId,
            AutoresIds = libro.LibroAutores.Select(la => la.AutorId)
        };

        patchDoc.ApplyTo(dto, ModelState);

        if (!TryValidateModel(dto))
            return ValidationProblem(ModelState);

        // Validate category if changed
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.CategoriaId = dto.CategoriaId;

        // Update authors only if provided in the patch
        if (dto.AutoresIds is not null)
        {
            var autores = await _context.Autores
                .Where(a => dto.AutoresIds.Contains(a.Id))
                .ToListAsync();

            if (autores.Count != dto.AutoresIds.Count())
                return BadRequest("Uno o más autores no fueron encontrados.");

            _context.LibroAutores.RemoveRange(libro.LibroAutores);
            foreach (var autor in autores)
            {
                _context.LibroAutores.Add(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un libro</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.LibroAutores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        _context.LibroAutores.RemoveRange(libro.LibroAutores);
        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<LibroDto?> BuildLibroDto(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return null;

        return new LibroDto(
            libro.Id,
            libro.Titulo,
            libro.Descripcion,
            libro.ISBN,
            libro.AnioPublicacion,
            libro.CategoriaId,
            libro.Categoria?.Nombre,
            libro.LibroAutores.Select(la => new AutorDto(
                la.Autor!.Id,
                la.Autor.Nombre,
                la.Autor.Apellido,
                la.Autor.Biografia
            ))
        );
    }
}
