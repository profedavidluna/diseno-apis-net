using LibreriaAPI.DTOs;
using LibreriaAPI.Models;

namespace LibreriaAPI.Repositories;

public interface ILibrosRepository
{
    Task<IEnumerable<LibroDto>> GetAllAsync();
    Task<LibroDto?> GetByIdAsync(int id);
    Task<IEnumerable<LibroDto>> GetByCategoriaAsync(int categoriaId);
    Task<Libro?> GetWithAutoresAsync(int id);
    Task<Categoria?> GetCategoriaAsync(int categoriaId);
    Task<List<Autor>> GetAutoresByIdsAsync(IEnumerable<int> ids);
    Task AddAsync(Libro libro);
    Task AddLibroAutorAsync(LibroAutor libroAutor);
    void RemoveLibroAutores(IEnumerable<LibroAutor> libroAutores);
    void Remove(Libro libro);
}
