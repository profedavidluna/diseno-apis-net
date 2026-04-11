using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriasRepository _repository;

    public CategoriasController(ICategoriasRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Obtiene todas las categorías</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        var categorias = await _repository.GetAllAsync();
        return Ok(categorias);
    }

    /// <summary>Obtiene una categoría por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
            return NotFound();

        return Ok(new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion));
    }

    /// <summary>Crea una nueva categoría</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaDto>> PostCategoria(CrearCategoriaDto dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        await _repository.AddAsync(categoria);
        await _repository.SaveChangesAsync();

        var result = new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, result);
    }

    /// <summary>Actualiza una categoría existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutCategoria(int id, ActualizarCategoriaDto dto)
    {
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
            return NotFound();

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        await _repository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina una categoría</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
            return NotFound();

        _repository.Remove(categoria);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
