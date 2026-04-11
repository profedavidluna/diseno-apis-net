using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Hateoas;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly LibreriaContext _context;
    private readonly IHateoasService<AutorDto> _hateoasService;

    public AutoresController(LibreriaContext context, IHateoasService<AutorDto> hateoasService)
    {
        _context = context;
        _hateoasService = hateoasService;
    }

    /// <summary>Obtiene todos los autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(HateoasResponse<IEnumerable<HateoasResponse<AutorDto>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<HateoasResponse<IEnumerable<HateoasResponse<AutorDto>>>>> GetAutores()
    {
        var autores = await _context.Autores
            .Select(a => new AutorDto(a.Id, a.Nombre, a.Apellido, a.Biografia))
            .ToListAsync();

        // Cada elemento lleva sus propios links
        var items = autores.Select(a =>
            new HateoasResponse<AutorDto>(a, _hateoasService.GenerateLinks(a, HttpContext)));

        // La colección también lleva links de nivel superior (self, create)
        var response = new HateoasResponse<IEnumerable<HateoasResponse<AutorDto>>>(
            items,
            _hateoasService.GenerateCollectionLinks(HttpContext));

        return Ok(response);
    }

    /// <summary>Obtiene un autor por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HateoasResponse<AutorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HateoasResponse<AutorDto>>> GetAutor(int id)
    {
        var autor = await _context.Autores.FindAsync(id);

        if (autor is null)
            return NotFound();

        var dto = new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
        var response = new HateoasResponse<AutorDto>(dto, _hateoasService.GenerateLinks(dto, HttpContext));

        return Ok(response);
    }

    /// <summary>Crea un nuevo autor</summary>
    [HttpPost]
    [ProducesResponseType(typeof(HateoasResponse<AutorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HateoasResponse<AutorDto>>> PostAutor(CrearAutorDto dto)
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
        var response = new HateoasResponse<AutorDto>(result, _hateoasService.GenerateLinks(result, HttpContext));

        return CreatedAtAction(nameof(GetAutor), new { id = autor.Id }, response);
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
