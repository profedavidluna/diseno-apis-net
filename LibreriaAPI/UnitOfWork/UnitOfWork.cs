using LibreriaAPI.Data;
using LibreriaAPI.Repositories;

namespace LibreriaAPI.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibreriaContext _context;

    public UnitOfWork(LibreriaContext context)
    {
        _context = context;
        Autores = new AutoresRepository(context);
        Categorias = new CategoriasRepository(context);
        Libros = new LibrosRepository(context);
    }

    public IAutoresRepository Autores { get; }
    public ICategoriasRepository Categorias { get; }
    public ILibrosRepository Libros { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
