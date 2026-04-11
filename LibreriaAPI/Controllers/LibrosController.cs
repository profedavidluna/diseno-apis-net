using LibreriaAPI.CQRS.Dispatcher;
using LibreriaAPI.CQRS.Libros.Commands;
using LibreriaAPI.CQRS.Libros.Queries;
using LibreriaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LibrosController : ControllerBase
{
    private readonly ICqrsDispatcher _dispatcher;

    public LibrosController(ICqrsDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>Obtiene todos los libros con su categoría y autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros(CancellationToken ct)
    {
        var libros = await _dispatcher.QueryAsync(new GetLibrosQuery(), ct);
        return Ok(libros);
    }

    /// <summary>Obtiene un libro por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> GetLibro(int id, CancellationToken ct)
    {
        var libro = await _dispatcher.QueryAsync(new GetLibroByIdQuery(id), ct);

        if (libro is null)
            return NotFound();

        return Ok(libro);
    }

    /// <summary>Obtiene los libros de una categoría específica</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibrosPorCategoria(int categoriaId, CancellationToken ct)
    {
        var libros = await _dispatcher.QueryAsync(new GetLibrosByCategoriaQuery(categoriaId), ct);
        return Ok(libros);
    }

    /// <summary>Crea un nuevo libro</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> PostLibro(CrearLibroDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync(
            new CreateLibroCommand(dto.Titulo, dto.Descripcion, dto.ISBN, dto.AnioPublicacion, dto.CategoriaId, dto.AutoresIds), ct);

        if (result.CategoriaNoEncontrada)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        if (result.AutoresInvalidos)
            return BadRequest("Uno o más autores no fueron encontrados.");

        return CreatedAtAction(nameof(GetLibro), new { id = result.Libro!.Id }, result.Libro);
    }

    /// <summary>Actualiza un libro existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLibro(int id, ActualizarLibroDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync(
            new UpdateLibroCommand(id, dto.Titulo, dto.Descripcion, dto.ISBN, dto.AnioPublicacion, dto.CategoriaId, dto.AutoresIds), ct);

        if (result.LibroNoEncontrado)
            return NotFound();

        if (result.CategoriaNoEncontrada)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        if (result.AutoresInvalidos)
            return BadRequest("Uno o más autores no fueron encontrados.");

        return NoContent();
    }

    /// <summary>Elimina un libro</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibro(int id, CancellationToken ct)
    {
        var found = await _dispatcher.CommandAsync(new DeleteLibroCommand(id), ct);

        if (!found)
            return NotFound();

        return NoContent();
    }
}

