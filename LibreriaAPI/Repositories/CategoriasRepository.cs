using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Repositories;

public class CategoriasRepository : ICategoriasRepository
{
    private readonly LibreriaContext _context;

    public CategoriasRepository(LibreriaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        => await _context.Categorias
            .Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion))
            .ToListAsync();

    public async Task<Categoria?> GetByIdAsync(int id)
        => await _context.Categorias.FindAsync(id);

    public async Task AddAsync(Categoria categoria)
        => await _context.Categorias.AddAsync(categoria);

    public void Remove(Categoria categoria)
        => _context.Categorias.Remove(categoria);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
