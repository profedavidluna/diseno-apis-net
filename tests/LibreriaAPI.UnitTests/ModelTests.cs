using FluentAssertions;
using LibreriaAPI.Models;

namespace LibreriaAPI.UnitTests;

public class ModelTests
{
    [Fact]
    public void Autor_PropiedadesDefault_SonCorrectas()
    {
        var autor = new Autor();

        autor.Id.Should().Be(0);
        autor.Nombre.Should().BeEmpty();
        autor.Apellido.Should().BeEmpty();
        autor.Biografia.Should().BeNull();
        autor.LibroAutores.Should().BeEmpty();
    }

    [Fact]
    public void Autor_AsignarPropiedades_FuncionaCorrectamente()
    {
        var autor = new Autor
        {
            Id = 1,
            Nombre = "Gabriel",
            Apellido = "García Márquez",
            Biografia = "Premio Nobel"
        };

        autor.Id.Should().Be(1);
        autor.Nombre.Should().Be("Gabriel");
        autor.Apellido.Should().Be("García Márquez");
        autor.Biografia.Should().Be("Premio Nobel");
    }

    [Fact]
    public void Categoria_PropiedadesDefault_SonCorrectas()
    {
        var categoria = new Categoria();

        categoria.Id.Should().Be(0);
        categoria.Nombre.Should().BeEmpty();
        categoria.Descripcion.Should().BeNull();
    }

    [Fact]
    public void Categoria_AsignarPropiedades_FuncionaCorrectamente()
    {
        var categoria = new Categoria
        {
            Id = 1,
            Nombre = "Ficción",
            Descripcion = "Libros de ficción literaria"
        };

        categoria.Id.Should().Be(1);
        categoria.Nombre.Should().Be("Ficción");
        categoria.Descripcion.Should().Be("Libros de ficción literaria");
    }

    [Fact]
    public void Libro_PropiedadesDefault_SonCorrectas()
    {
        var libro = new Libro();

        libro.Id.Should().Be(0);
        libro.Titulo.Should().BeEmpty();
        libro.Descripcion.Should().BeNull();
        libro.ISBN.Should().BeNull();
        libro.AnioPublicacion.Should().Be(0);
        libro.LibroAutores.Should().BeEmpty();
    }

    [Fact]
    public void Libro_AsignarPropiedades_FuncionaCorrectamente()
    {
        var libro = new Libro
        {
            Id = 1,
            Titulo = "Cien años de soledad",
            ISBN = "978-0-06-088328-7",
            AnioPublicacion = 1967,
            CategoriaId = 1
        };

        libro.Id.Should().Be(1);
        libro.Titulo.Should().Be("Cien años de soledad");
        libro.ISBN.Should().Be("978-0-06-088328-7");
        libro.AnioPublicacion.Should().Be(1967);
        libro.CategoriaId.Should().Be(1);
    }

    [Fact]
    public void LibroAutor_AsignarPropiedades_FuncionaCorrectamente()
    {
        var libroAutor = new LibroAutor
        {
            LibroId = 1,
            AutorId = 2
        };

        libroAutor.LibroId.Should().Be(1);
        libroAutor.AutorId.Should().Be(2);
    }

    [Fact]
    public void Autor_ColeccionLibroAutores_EsInicialmenteVacia()
    {
        var autor = new Autor();

        autor.LibroAutores.Should().NotBeNull();
        autor.LibroAutores.Should().BeEmpty();
    }

    [Fact]
    public void Libro_ColeccionLibroAutores_EsInicialmenteVacia()
    {
        var libro = new Libro();

        libro.LibroAutores.Should().NotBeNull();
        libro.LibroAutores.Should().BeEmpty();
    }
}
