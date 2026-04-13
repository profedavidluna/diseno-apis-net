using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly LibreriaContext _context;

    public AutoresController(LibreriaContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene todos los autores</summary>
    [HttpGet]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(IEnumerable<AutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorDto>>> GetAutores()
    {
        var autores = await _context.Autores
            .Select(a => new AutorDto(a.Id, a.Nombre, a.Apellido, a.Biografia))
            .ToListAsync();

        return Ok(autores);
    }

    /// <summary>Obtiene un autor por su ID</summary>
    [HttpGet("{id:int}")]
    [ResponseCache(Duration = 60, VaryByHeader = "Accept")]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorDto>> GetAutor(int id)
    {
        var autor = await _context.Autores.FindAsync(id);

        if (autor is null)
            return NotFound();

        return Ok(new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia));
    }

    /// <summary>Crea un nuevo autor</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutorDto>> PostAutor(CrearAutorDto dto)
    {
        var autor = new Autor
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Biografia = dto.Biografia
        };

        _context.Autores.Add(autor);
        await _context.SaveChangesAsync();

        var result = new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
        return CreatedAtAction(nameof(GetAutor), new { id = autor.Id }, result);
    }

    /// <summary>Actualiza un autor existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAutor(int id, ActualizarAutorDto dto)
    {
        var autor = await _context.Autores.FindAsync(id);

        if (autor is null)
            return NotFound();

        autor.Nombre = dto.Nombre;
        autor.Apellido = dto.Apellido;
        autor.Biografia = dto.Biografia;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Actualiza parcialmente un autor usando JSON Patch (RFC 6902)</summary>
    /// <remarks>
    /// Ejemplo de body (application/json-patch+json):
    ///
    ///     [
    ///       { "op": "replace", "path": "/nombre", "value": "Nuevo Nombre" }
    ///     ]
    /// </remarks>
    [HttpPatch("{id:int}")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchAutor(int id, JsonPatchDocument<PatchAutorDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest();

        var autor = await _context.Autores.FindAsync(id);

        if (autor is null)
            return NotFound();

        var dto = new PatchAutorDto
        {
            Nombre = autor.Nombre,
            Apellido = autor.Apellido,
            Biografia = autor.Biografia
        };

        patchDoc.ApplyTo(dto, ModelState);

        if (!TryValidateModel(dto))
            return ValidationProblem(ModelState);

        autor.Nombre = dto.Nombre;
        autor.Apellido = dto.Apellido;
        autor.Biografia = dto.Biografia;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un autor</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAutor(int id)
    {
        var autor = await _context.Autores.FindAsync(id);

        if (autor is null)
            return NotFound();

        _context.Autores.Remove(autor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
