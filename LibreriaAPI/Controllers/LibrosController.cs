using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LibrosController : ControllerBase
{
    private readonly ILibrosRepository _repository;

    public LibrosController(ILibrosRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Obtiene todos los libros con su categoría y autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
    {
        var libros = await _repository.GetAllAsync();
        return Ok(libros);
    }

    /// <summary>Obtiene un libro por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> GetLibro(int id)
    {
        var libro = await _repository.GetByIdAsync(id);

        if (libro is null)
            return NotFound();

        return Ok(libro);
    }

    /// <summary>Obtiene los libros de una categoría específica</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibrosPorCategoria(int categoriaId)
    {
        var libros = await _repository.GetByCategoriaAsync(categoriaId);
        return Ok(libros);
    }

    /// <summary>Crea un nuevo libro</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> PostLibro(CrearLibroDto dto)
    {
        var categoria = await _repository.GetCategoriaAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _repository.GetAutoresByIdsAsync(dto.AutoresIds);

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

        await _repository.AddAsync(libro);
        await _repository.SaveChangesAsync();

        foreach (var autor in autores)
        {
            await _repository.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }
        await _repository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, await _repository.GetByIdAsync(libro.Id));
    }

    /// <summary>Actualiza un libro existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLibro(int id, ActualizarLibroDto dto)
    {
        var libro = await _repository.GetWithAutoresAsync(id);

        if (libro is null)
            return NotFound();

        var categoria = await _repository.GetCategoriaAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _repository.GetAutoresByIdsAsync(dto.AutoresIds);

        if (autores.Count != dto.AutoresIds.Count())
            return BadRequest("Uno o más autores no fueron encontrados.");

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.CategoriaId = dto.CategoriaId;

        _repository.RemoveLibroAutores(libro.LibroAutores);
        foreach (var autor in autores)
        {
            await _repository.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }

        await _repository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un libro</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var libro = await _repository.GetWithAutoresAsync(id);

        if (libro is null)
            return NotFound();

        _repository.RemoveLibroAutores(libro.LibroAutores);
        _repository.Remove(libro);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
