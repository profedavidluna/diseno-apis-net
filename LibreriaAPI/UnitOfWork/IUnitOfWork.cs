using LibreriaAPI.Repositories;

namespace LibreriaAPI.UnitOfWork;

public interface IUnitOfWork
{
    IAutoresRepository Autores { get; }
    ICategoriasRepository Categorias { get; }
    ILibrosRepository Libros { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
