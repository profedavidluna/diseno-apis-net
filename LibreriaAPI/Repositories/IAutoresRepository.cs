using LibreriaAPI.DTOs;
using LibreriaAPI.Models;

namespace LibreriaAPI.Repositories;

public interface IAutoresRepository
{
    Task<IEnumerable<AutorDto>> GetAllAsync();
    Task<Autor?> GetByIdAsync(int id);
    Task AddAsync(Autor autor);
    void Remove(Autor autor);
}
