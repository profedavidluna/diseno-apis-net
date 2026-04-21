using LibreriaAPI.DTOs;

namespace LibreriaAPI.Repositories;

public interface IAutoresReadRepository
{
    Task<IEnumerable<AutorDto>> GetAllAsync();
    Task<AutorDto?> GetByIdAsync(int id);
}
