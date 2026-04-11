using LibreriaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Data;

public class LibreriaContext : DbContext
{
    public LibreriaContext(DbContextOptions<LibreriaContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<LibroAutor> LibroAutores => Set<LibroAutor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite primary key for the join table
        modelBuilder.Entity<LibroAutor>()
            .HasKey(la => new { la.LibroId, la.AutorId });

        modelBuilder.Entity<LibroAutor>()
            .HasOne(la => la.Libro)
            .WithMany(l => l.LibroAutores)
            .HasForeignKey(la => la.LibroId);

        modelBuilder.Entity<LibroAutor>()
            .HasOne(la => la.Autor)
            .WithMany(a => a.LibroAutores)
            .HasForeignKey(la => la.AutorId);

        // Seed data
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Ficción", Descripcion = "Libros de ficción literaria" },
            new Categoria { Id = 2, Nombre = "Ciencia", Descripcion = "Libros de ciencia y divulgación científica" },
            new Categoria { Id = 3, Nombre = "Historia", Descripcion = "Libros de historia y biografías" }
        );

        modelBuilder.Entity<Autor>().HasData(
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Biografia = "Escritor colombiano, Premio Nobel de Literatura 1982" },
            new Autor { Id = 2, Nombre = "Yuval Noah", Apellido = "Harari", Biografia = "Historiador y escritor israelí" },
            new Autor { Id = 3, Nombre = "Isabel", Apellido = "Allende", Biografia = "Escritora chilena de novelas históricas y de ficción" }
        );

        modelBuilder.Entity<Libro>().HasData(
            new Libro { Id = 1, Titulo = "Cien años de soledad", ISBN = "978-0-06-088328-7", AnioPublicacion = 1967, CategoriaId = 1, Descripcion = "La historia de la familia Buendía en el pueblo ficticio de Macondo" },
            new Libro { Id = 2, Titulo = "Sapiens: De animales a dioses", ISBN = "978-0-06-231609-7", AnioPublicacion = 2011, CategoriaId = 2, Descripcion = "Una breve historia de la humanidad" },
            new Libro { Id = 3, Titulo = "La casa de los espíritus", ISBN = "978-0-15-147871-1", AnioPublicacion = 1982, CategoriaId = 1, Descripcion = "Saga familiar de los Trueba en un país latinoamericano" }
        );

        modelBuilder.Entity<LibroAutor>().HasData(
            new LibroAutor { LibroId = 1, AutorId = 1 },
            new LibroAutor { LibroId = 2, AutorId = 2 },
            new LibroAutor { LibroId = 3, AutorId = 3 }
        );
    }
}
