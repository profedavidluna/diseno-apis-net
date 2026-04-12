using LibreriaAPI.CQRS.Categorias.Commands;
using LibreriaAPI.CQRS.Categorias.Queries;
using LibreriaAPI.CQRS.Dispatcher;
using LibreriaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly ICqrsDispatcher _dispatcher;

    public CategoriasController(ICqrsDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>Obtiene todas las categorías</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias(CancellationToken ct)
    {
        var categorias = await _dispatcher.QueryAsync(new GetCategoriasQuery(), ct);
        return Ok(categorias);
    }

    /// <summary>Obtiene una categoría por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id, CancellationToken ct)
    {
        var categoria = await _dispatcher.QueryAsync(new GetCategoriaByIdQuery(id), ct);

        if (categoria is null)
            return NotFound();

        return Ok(categoria);
    }

    /// <summary>Crea una nueva categoría</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaDto>> PostCategoria(CrearCategoriaDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync(
            new CreateCategoriaCommand(dto.Nombre, dto.Descripcion), ct);

        return CreatedAtAction(nameof(GetCategoria), new { id = result.Id }, result);
    }

    /// <summary>Actualiza una categoría existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutCategoria(int id, ActualizarCategoriaDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync(
            new UpdateCategoriaCommand(id, dto.Nombre, dto.Descripcion), ct);

        if (result is null)
            return NotFound();

        return NoContent();
    }

    /// <summary>Elimina una categoría</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoria(int id, CancellationToken ct)
    {
        var found = await _dispatcher.CommandAsync(new DeleteCategoriaCommand(id), ct);

        if (!found)
            return NotFound();

        return NoContent();
    }
}

