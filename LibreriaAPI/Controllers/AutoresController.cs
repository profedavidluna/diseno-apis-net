using LibreriaAPI.DTOs;
using LibreriaAPI.Features.Autores.Commands;
using LibreriaAPI.Features.Autores.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public AutoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene todos los autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorDto>>> GetAutores()
    {
        var autores = await _mediator.Send(new GetAutoresQuery());
        return Ok(autores);
    }

    /// <summary>Obtiene un autor por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorDto>> GetAutor(int id)
    {
        var autor = await _mediator.Send(new GetAutorByIdQuery(id));

        if (autor is null)
            return NotFound();

        return Ok(autor);
    }

    /// <summary>Crea un nuevo autor</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutorDto>> PostAutor(CrearAutorDto dto)
    {
        var result = await _mediator.Send(new CrearAutorCommand(dto.Nombre, dto.Apellido, dto.Biografia));
        return CreatedAtAction(nameof(GetAutor), new { id = result.Id }, result);
    }

    /// <summary>Actualiza un autor existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAutor(int id, ActualizarAutorDto dto)
    {
        var found = await _mediator.Send(new ActualizarAutorCommand(id, dto.Nombre, dto.Apellido, dto.Biografia));

        if (!found)
            return NotFound();

        return NoContent();
    }

    /// <summary>Elimina un autor</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAutor(int id)
    {
        var found = await _mediator.Send(new EliminarAutorCommand(id));

        if (!found)
            return NotFound();

        return NoContent();
    }
}
