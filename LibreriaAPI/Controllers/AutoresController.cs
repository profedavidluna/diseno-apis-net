using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public AutoresController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    /// <summary>Obtiene todos los autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorDto>>> GetAutores()
    {
        var autores = await _uow.Autores.GetAllAsync();
        return Ok(autores);
    }

    /// <summary>Obtiene un autor por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AutorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorDto>> GetAutor(int id)
    {
        var autor = await _uow.Autores.GetByIdAsync(id);

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

        await _uow.Autores.AddAsync(autor);
        await _uow.SaveChangesAsync();

        var result = new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
        return CreatedAtAction(nameof(GetAutor), new { id = autor.Id }, result);
    }

    /// <summary>Actualiza un autor existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAutor(int id, ActualizarAutorDto dto)
    {
        var autor = await _uow.Autores.GetByIdAsync(id);

        if (autor is null)
            return NotFound();

        autor.Nombre = dto.Nombre;
        autor.Apellido = dto.Apellido;
        autor.Biografia = dto.Biografia;

        await _uow.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un autor</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAutor(int id)
    {
        var autor = await _uow.Autores.GetByIdAsync(id);

        if (autor is null)
            return NotFound();

        _uow.Autores.Remove(autor);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

