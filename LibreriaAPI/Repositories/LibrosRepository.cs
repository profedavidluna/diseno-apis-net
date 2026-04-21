using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Repositories;

public class LibrosRepository : ILibrosRepository
{
    private readonly LibreriaContext _context;

    public LibrosRepository(LibreriaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LibroDto>> GetAllAsync()
        => await _context.Libros
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

    public async Task<LibroDto?> GetByIdAsync(int id)
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

    public async Task<IEnumerable<LibroDto>> GetByCategoriaAsync(int categoriaId)
        => await _context.Libros
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

    public async Task<Libro?> GetWithAutoresAsync(int id)
        => await _context.Libros
            .Include(l => l.LibroAutores)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Categoria?> GetCategoriaAsync(int categoriaId)
        => await _context.Categorias.FindAsync(categoriaId);

    public async Task<List<Autor>> GetAutoresByIdsAsync(IEnumerable<int> ids)
        => await _context.Autores
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();

    public async Task AddAsync(Libro libro)
        => await _context.Libros.AddAsync(libro);

    public async Task AddLibroAutorAsync(LibroAutor libroAutor)
        => await _context.LibroAutores.AddAsync(libroAutor);

    public void RemoveLibroAutores(IEnumerable<LibroAutor> libroAutores)
        => _context.LibroAutores.RemoveRange(libroAutores);

    public void Remove(Libro libro)
        => _context.Libros.Remove(libro);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
