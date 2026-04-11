using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LibrosController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public LibrosController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    /// <summary>Obtiene todos los libros con su categoría y autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
    {
        var libros = await _uow.Libros.GetAllAsync();
        return Ok(libros);
    }

    /// <summary>Obtiene un libro por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> GetLibro(int id)
    {
        var libro = await _uow.Libros.GetByIdAsync(id);

        if (libro is null)
            return NotFound();

        return Ok(libro);
    }

    /// <summary>Obtiene los libros de una categoría específica</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibrosPorCategoria(int categoriaId)
    {
        var libros = await _uow.Libros.GetByCategoriaAsync(categoriaId);
        return Ok(libros);
    }

    /// <summary>Crea un nuevo libro</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> PostLibro(CrearLibroDto dto)
    {
        var categoria = await _uow.Libros.GetCategoriaAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _uow.Libros.GetAutoresByIdsAsync(dto.AutoresIds);

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

        await _uow.Libros.AddAsync(libro);
        await _uow.SaveChangesAsync();

        foreach (var autor in autores)
        {
            await _uow.Libros.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }
        await _uow.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, await _uow.Libros.GetByIdAsync(libro.Id));
    }

    /// <summary>Actualiza un libro existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLibro(int id, ActualizarLibroDto dto)
    {
        var libro = await _uow.Libros.GetWithAutoresAsync(id);

        if (libro is null)
            return NotFound();

        var categoria = await _uow.Libros.GetCategoriaAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _uow.Libros.GetAutoresByIdsAsync(dto.AutoresIds);

        if (autores.Count != dto.AutoresIds.Count())
            return BadRequest("Uno o más autores no fueron encontrados.");

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.CategoriaId = dto.CategoriaId;

        _uow.Libros.RemoveLibroAutores(libro.LibroAutores);
        foreach (var autor in autores)
        {
            await _uow.Libros.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }

        await _uow.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un libro</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var libro = await _uow.Libros.GetWithAutoresAsync(id);

        if (libro is null)
            return NotFound();

        _uow.Libros.RemoveLibroAutores(libro.LibroAutores);
        _uow.Libros.Remove(libro);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

