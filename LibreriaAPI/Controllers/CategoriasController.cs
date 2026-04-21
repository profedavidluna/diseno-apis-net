using LibreriaAPI.DTOs;
using LibreriaAPI.Features.Categorias.Commands;
using LibreriaAPI.Features.Categorias.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene todas las categorías</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        var categorias = await _mediator.Send(new GetCategoriasQuery());
        return Ok(categorias);
    }

    /// <summary>Obtiene una categoría por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _mediator.Send(new GetCategoriaByIdQuery(id));

        if (categoria is null)
            return NotFound();

        return Ok(categoria);
    }

    /// <summary>Crea una nueva categoría</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaDto>> PostCategoria(CrearCategoriaDto dto)
    {
        var result = await _mediator.Send(new CrearCategoriaCommand(dto.Nombre, dto.Descripcion));
        return CreatedAtAction(nameof(GetCategoria), new { id = result.Id }, result);
    }

    /// <summary>Actualiza una categoría existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutCategoria(int id, ActualizarCategoriaDto dto)
    {
        var found = await _mediator.Send(new ActualizarCategoriaCommand(id, dto.Nombre, dto.Descripcion));

        if (!found)
            return NotFound();

        return NoContent();
    }

    /// <summary>Elimina una categoría</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var found = await _mediator.Send(new EliminarCategoriaCommand(id));

        if (!found)
            return NotFound();

        return NoContent();
    }
}
