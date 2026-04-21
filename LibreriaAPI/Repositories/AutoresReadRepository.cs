using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Repositories;

public class AutoresReadRepository : IAutoresReadRepository
{
    private readonly LibreriaReadContext _context;

    public AutoresReadRepository(LibreriaReadContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AutorDto>> GetAllAsync()
        => await _context.Autores
            .Select(a => new AutorDto(a.Id, a.Nombre, a.Apellido, a.Biografia))
            .ToListAsync();

    public async Task<AutorDto?> GetByIdAsync(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        return autor is null ? null : new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
    }
}
