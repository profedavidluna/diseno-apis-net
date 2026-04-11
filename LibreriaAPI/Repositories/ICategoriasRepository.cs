using LibreriaAPI.DTOs;
using LibreriaAPI.Models;

namespace LibreriaAPI.Repositories;

public interface ICategoriasRepository
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    Task<Categoria?> GetByIdAsync(int id);
    Task AddAsync(Categoria categoria);
    void Remove(Categoria categoria);
}
