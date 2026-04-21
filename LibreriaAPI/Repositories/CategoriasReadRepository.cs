using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Repositories;

public class CategoriasReadRepository : ICategoriasReadRepository
{
    private readonly LibreriaReadContext _context;

    public CategoriasReadRepository(LibreriaReadContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        => await _context.Categorias
            .Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion))
            .ToListAsync();

    public async Task<CategoriaDto?> GetByIdAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        return categoria is null ? null : new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
