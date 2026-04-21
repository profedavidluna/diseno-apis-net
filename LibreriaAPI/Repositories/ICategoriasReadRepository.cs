using LibreriaAPI.DTOs;

namespace LibreriaAPI.Repositories;

public interface ICategoriasReadRepository
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(int id);
}
