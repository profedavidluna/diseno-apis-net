using LibreriaAPI.DTOs;

namespace LibreriaAPI.Repositories;

public interface ILibrosReadRepository
{
    Task<IEnumerable<LibroDto>> GetAllAsync();
    Task<LibroDto?> GetByIdAsync(int id);
    Task<IEnumerable<LibroDto>> GetByCategoriaAsync(int categoriaId);
}
