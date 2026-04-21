using LibreriaAPI.DTOs;
using LibreriaAPI.Features.Libros.Commands;
using LibreriaAPI.Features.Libros.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LibrosController : ControllerBase
{
    private readonly IMediator _mediator;

    public LibrosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene todos los libros con su categoría y autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
    {
        var libros = await _mediator.Send(new GetLibrosQuery());
        return Ok(libros);
    }

    /// <summary>Obtiene un libro por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> GetLibro(int id)
    {
        var libro = await _mediator.Send(new GetLibroByIdQuery(id));

        if (libro is null)
            return NotFound();

        return Ok(libro);
    }

    /// <summary>Obtiene los libros de una categoría específica</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LibroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibrosPorCategoria(int categoriaId)
    {
        var libros = await _mediator.Send(new GetLibrosPorCategoriaQuery(categoriaId));
        return Ok(libros);
    }

    /// <summary>Crea un nuevo libro</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LibroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibroDto>> PostLibro(CrearLibroDto dto)
    {
        var result = await _mediator.Send(new CrearLibroCommand(
            dto.Titulo, dto.Descripcion, dto.ISBN, dto.AnioPublicacion, dto.CategoriaId, dto.AutoresIds));

        if (result.Error is not null)
            return result.Libro is null ? NotFound(result.Error) : BadRequest(result.Error);

        return CreatedAtAction(nameof(GetLibro), new { id = result.Libro!.Id }, result.Libro);
    }

    /// <summary>Actualiza un libro existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLibro(int id, ActualizarLibroDto dto)
    {
        var result = await _mediator.Send(new ActualizarLibroCommand(
            id, dto.Titulo, dto.Descripcion, dto.ISBN, dto.AnioPublicacion, dto.CategoriaId, dto.AutoresIds));

        if (!result.Encontrado)
            return NotFound();

        if (result.Error is not null)
            return BadRequest(result.Error);

        return NoContent();
    }

    /// <summary>Elimina un libro</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var found = await _mediator.Send(new EliminarLibroCommand(id));

        if (!found)
            return NotFound();

        return NoContent();
    }
}
