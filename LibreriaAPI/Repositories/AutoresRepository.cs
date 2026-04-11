using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Repositories;

public class AutoresRepository : IAutoresRepository
{
    private readonly LibreriaContext _context;

    public AutoresRepository(LibreriaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AutorDto>> GetAllAsync()
        => await _context.Autores
            .Select(a => new AutorDto(a.Id, a.Nombre, a.Apellido, a.Biografia))
            .ToListAsync();

    public async Task<Autor?> GetByIdAsync(int id)
        => await _context.Autores.FindAsync(id);

    public async Task AddAsync(Autor autor)
        => await _context.Autores.AddAsync(autor);

    public void Remove(Autor autor)
        => _context.Autores.Remove(autor);
}
