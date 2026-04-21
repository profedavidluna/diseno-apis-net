using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibreriaAPI.Repositories;

public class LibrosReadRepository : ILibrosReadRepository
{
    private readonly LibreriaReadContext _context;

    public LibrosReadRepository(LibreriaReadContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LibroDto>> GetAllAsync()
    {
        var readModels = await _context.Libros.ToListAsync();
        return readModels.Select(MapToDto);
    }

    public async Task<LibroDto?> GetByIdAsync(int id)
    {
        var readModel = await _context.Libros.FindAsync(id);
        return readModel is null ? null : MapToDto(readModel);
    }

    public async Task<IEnumerable<LibroDto>> GetByCategoriaAsync(int categoriaId)
    {
        var readModels = await _context.Libros
            .Where(l => l.CategoriaId == categoriaId)
            .ToListAsync();
        return readModels.Select(MapToDto);
    }

    private static LibroDto MapToDto(LibroReadModel model)
    {
        var autores = JsonSerializer.Deserialize<IEnumerable<AutorDto>>(model.AutoresJson)
                      ?? Enumerable.Empty<AutorDto>();
        return new LibroDto(
            model.Id,
            model.Titulo,
            model.Descripcion,
            model.ISBN,
            model.AnioPublicacion,
            model.CategoriaId,
            model.CategoriaNombre,
            autores);
    }
}
