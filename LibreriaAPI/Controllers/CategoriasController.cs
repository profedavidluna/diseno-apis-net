using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly LibreriaContext _context;

    public CategoriasController(LibreriaContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene todas las categorías</summary>
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        var categorias = await _context.Categorias
            .Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion))
            .ToListAsync();

        return Ok(categorias);
    }

    /// <summary>Obtiene una categoría por su ID</summary>
    [HttpGet("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria is null)
            return NotFound();

        return Ok(new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion));
    }

    /// <summary>Crea una nueva categoría</summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoriaDto>> PostCategoria(CrearCategoriaDto dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        var result = new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, result);
    }

    /// <summary>Actualiza una categoría existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutCategoria(int id, ActualizarCategoriaDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria is null)
            return NotFound();

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Actualiza parcialmente una categoría usando JSON Patch (RFC 6902)</summary>
    /// <remarks>
    /// Ejemplo de body (application/json-patch+json):
    ///
    ///     [
    ///       { "op": "replace", "path": "/nombre", "value": "Nueva Categoría" }
    ///     ]
    /// </remarks>
    [HttpPatch("{id:int}")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchCategoria(int id, JsonPatchDocument<PatchCategoriaDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest();

        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria is null)
            return NotFound();

        var dto = new PatchCategoriaDto
        {
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion
        };

        patchDoc.ApplyTo(dto, ModelState);

        if (!TryValidateModel(dto))
            return ValidationProblem(ModelState);

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina una categoría</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria is null)
            return NotFound();

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
