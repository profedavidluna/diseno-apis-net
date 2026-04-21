using LibreriaAPI.Models.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Data;

/// <summary>
/// BD de lectura (Read DB): almacena proyecciones/modelos desnormalizados
/// sincronizados desde la BD de escritura mediante eventos de dominio.
/// </summary>
public class LibreriaReadContext : DbContext
{
    public LibreriaReadContext(DbContextOptions<LibreriaReadContext> options) : base(options) { }

    public DbSet<LibroReadModel> Libros => Set<LibroReadModel>();
    public DbSet<AutorReadModel> Autores => Set<AutorReadModel>();
    public DbSet<CategoriaReadModel> Categorias => Set<CategoriaReadModel>();
    public DbSet<LibroAutorReadModel> LibroAutores => Set<LibroAutorReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LibroAutorReadModel>()
            .HasKey(la => new { la.LibroId, la.AutorId });
    }
}
