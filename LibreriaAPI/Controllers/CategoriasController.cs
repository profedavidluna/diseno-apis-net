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
public class CategoriasController : ControllerBase
{
    private readonly LibreriaContext _context;
    private readonly IHateoasService<CategoriaDto> _hateoasService;

    public CategoriasController(LibreriaContext context, IHateoasService<CategoriaDto> hateoasService)
    {
        _context = context;
        _hateoasService = hateoasService;
    }

    /// <summary>Obtiene todas las categorías</summary>
    [HttpGet]
    [ProducesResponseType(typeof(HateoasResponse<IEnumerable<HateoasResponse<CategoriaDto>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<HateoasResponse<IEnumerable<HateoasResponse<CategoriaDto>>>>> GetCategorias()
    {
        var categorias = await _context.Categorias
            .Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion))
            .ToListAsync();

        var items = categorias.Select(c =>
            new HateoasResponse<CategoriaDto>(c, _hateoasService.GenerateLinks(c, HttpContext)));

        var response = new HateoasResponse<IEnumerable<HateoasResponse<CategoriaDto>>>(
            items,
            _hateoasService.GenerateCollectionLinks(HttpContext));

        return Ok(response);
    }

    /// <summary>Obtiene una categoría por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HateoasResponse<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HateoasResponse<CategoriaDto>>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria is null)
            return NotFound();

        var dto = new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
        var response = new HateoasResponse<CategoriaDto>(dto, _hateoasService.GenerateLinks(dto, HttpContext));

        return Ok(response);
    }

    /// <summary>Crea una nueva categoría</summary>
    [HttpPost]
    [ProducesResponseType(typeof(HateoasResponse<CategoriaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HateoasResponse<CategoriaDto>>> PostCategoria(CrearCategoriaDto dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        var result = new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
        var response = new HateoasResponse<CategoriaDto>(result, _hateoasService.GenerateLinks(result, HttpContext));

        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, response);
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
