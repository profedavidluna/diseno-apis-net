using LibreriaAPI.CQRS.Autores.Commands;
using LibreriaAPI.CQRS.Autores.Queries;
using LibreriaAPI.CQRS.Dispatcher;
using LibreriaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly ICqrsDispatcher _dispatcher;

    public AutoresController(ICqrsDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>Obtiene todos los autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorDto>>> GetAutores(CancellationToken ct)
    {
        var autores = await _dispatcher.QueryAsync(new GetAutoresQuery(), ct);
        return Ok(autores);
    }

    /// <summary>Obtiene un autor por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorDto>> GetAutor(int id, CancellationToken ct)
    {
        var autor = await _dispatcher.QueryAsync(new GetAutorByIdQuery(id), ct);

        if (autor is null)
            return NotFound();

        return Ok(autor);
    }

    /// <summary>Crea un nuevo autor</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutorDto>> PostAutor(CrearAutorDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync(
            new CreateAutorCommand(dto.Nombre, dto.Apellido, dto.Biografia), ct);

        return CreatedAtAction(nameof(GetAutor), new { id = result.Id }, result);
    }

    /// <summary>Actualiza un autor existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAutor(int id, ActualizarAutorDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync(
            new UpdateAutorCommand(id, dto.Nombre, dto.Apellido, dto.Biografia), ct);

        if (result is null)
            return NotFound();

        return NoContent();
    }

    /// <summary>Elimina un autor</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAutor(int id, CancellationToken ct)
    {
        var found = await _dispatcher.CommandAsync(new DeleteAutorCommand(id), ct);

        if (!found)
            return NotFound();

        return NoContent();
    }
}

