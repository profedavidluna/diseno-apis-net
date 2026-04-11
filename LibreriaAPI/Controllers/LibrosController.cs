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
public class LibrosController : ControllerBase
{
    private readonly LibreriaContext _context;
    private readonly IHateoasService<LibroDto> _hateoasService;

    public LibrosController(LibreriaContext context, IHateoasService<LibroDto> hateoasService)
    {
        _context = context;
        _hateoasService = hateoasService;
    }

    /// <summary>Obtiene todos los libros con su categoría y autores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(HateoasResponse<IEnumerable<HateoasResponse<LibroDto>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<HateoasResponse<IEnumerable<HateoasResponse<LibroDto>>>>> GetLibros()
    {
        var libros = await _context.Libros
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .Select(l => new LibroDto(
                l.Id,
                l.Titulo,
                l.Descripcion,
                l.ISBN,
                l.AnioPublicacion,
                l.CategoriaId,
                l.Categoria != null ? l.Categoria.Nombre : null,
                l.LibroAutores.Select(la => new AutorDto(
                    la.Autor!.Id,
                    la.Autor.Nombre,
                    la.Autor.Apellido,
                    la.Autor.Biografia
                ))
            ))
            .ToListAsync();

        var items = libros.Select(l =>
            new HateoasResponse<LibroDto>(l, _hateoasService.GenerateLinks(l, HttpContext)));

        var response = new HateoasResponse<IEnumerable<HateoasResponse<LibroDto>>>(
            items,
            _hateoasService.GenerateCollectionLinks(HttpContext));

        return Ok(response);
    }

    /// <summary>Obtiene un libro por su ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HateoasResponse<LibroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HateoasResponse<LibroDto>>> GetLibro(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        var dto = new LibroDto(
            libro.Id,
            libro.Titulo,
            libro.Descripcion,
            libro.ISBN,
            libro.AnioPublicacion,
            libro.CategoriaId,
            libro.Categoria?.Nombre,
            libro.LibroAutores.Select(la => new AutorDto(
                la.Autor!.Id,
                la.Autor.Nombre,
                la.Autor.Apellido,
                la.Autor.Biografia
            ))
        );

        var response = new HateoasResponse<LibroDto>(dto, _hateoasService.GenerateLinks(dto, HttpContext));
        return Ok(response);
    }

    /// <summary>Obtiene los libros de una categoría específica</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(HateoasResponse<IEnumerable<HateoasResponse<LibroDto>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<HateoasResponse<IEnumerable<HateoasResponse<LibroDto>>>>> GetLibrosPorCategoria(int categoriaId)
    {
        var libros = await _context.Libros
            .Where(l => l.CategoriaId == categoriaId)
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .Select(l => new LibroDto(
                l.Id,
                l.Titulo,
                l.Descripcion,
                l.ISBN,
                l.AnioPublicacion,
                l.CategoriaId,
                l.Categoria != null ? l.Categoria.Nombre : null,
                l.LibroAutores.Select(la => new AutorDto(
                    la.Autor!.Id,
                    la.Autor.Nombre,
                    la.Autor.Apellido,
                    la.Autor.Biografia
                ))
            ))
            .ToListAsync();

        var items = libros.Select(l =>
            new HateoasResponse<LibroDto>(l, _hateoasService.GenerateLinks(l, HttpContext)));

        var response = new HateoasResponse<IEnumerable<HateoasResponse<LibroDto>>>(
            items,
            _hateoasService.GenerateCollectionLinks(HttpContext));

        return Ok(response);
    }

    /// <summary>Crea un nuevo libro</summary>
    [HttpPost]
    [ProducesResponseType(typeof(HateoasResponse<LibroDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HateoasResponse<LibroDto>>> PostLibro(CrearLibroDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _context.Autores
            .Where(a => dto.AutoresIds.Contains(a.Id))
            .ToListAsync();

        if (autores.Count != dto.AutoresIds.Count())
            return BadRequest("Uno o más autores no fueron encontrados.");

        var libro = new Libro
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            ISBN = dto.ISBN,
            AnioPublicacion = dto.AnioPublicacion,
            CategoriaId = dto.CategoriaId
        };

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        foreach (var autor in autores)
        {
            _context.LibroAutores.Add(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }
        await _context.SaveChangesAsync();

        var libroDto = await BuildLibroDto(libro.Id);
        if (libroDto is null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al recuperar el libro creado.");

        var response = new HateoasResponse<LibroDto>(libroDto, _hateoasService.GenerateLinks(libroDto, HttpContext));

        return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, response);
    }

    /// <summary>Actualiza un libro existente</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLibro(int id, ActualizarLibroDto dto)
    {
        var libro = await _context.Libros
            .Include(l => l.LibroAutores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return NotFound($"Categoría con Id {dto.CategoriaId} no encontrada.");

        var autores = await _context.Autores
            .Where(a => dto.AutoresIds.Contains(a.Id))
            .ToListAsync();

        if (autores.Count != dto.AutoresIds.Count())
            return BadRequest("Uno o más autores no fueron encontrados.");

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.CategoriaId = dto.CategoriaId;

        _context.LibroAutores.RemoveRange(libro.LibroAutores);
        foreach (var autor in autores)
        {
            _context.LibroAutores.Add(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un libro</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.LibroAutores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return NotFound();

        _context.LibroAutores.RemoveRange(libro.LibroAutores);
        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<LibroDto?> BuildLibroDto(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Categoria)
            .Include(l => l.LibroAutores)
                .ThenInclude(la => la.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro is null)
            return null;

        return new LibroDto(
            libro.Id,
            libro.Titulo,
            libro.Descripcion,
            libro.ISBN,
            libro.AnioPublicacion,
            libro.CategoriaId,
            libro.Categoria?.Nombre,
            libro.LibroAutores.Select(la => new AutorDto(
                la.Autor!.Id,
                la.Autor.Nombre,
                la.Autor.Apellido,
                la.Autor.Biografia
            ))
        );
    }
}
