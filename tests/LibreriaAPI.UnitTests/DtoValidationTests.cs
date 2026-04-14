using FluentAssertions;
using LibreriaAPI.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LibreriaAPI.UnitTests;

public class DtoValidationTests
{
    private static IList<ValidationResult> Validate(object obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj);
        Validator.TryValidateObject(obj, context, results, true);
        return results;
    }

    // ── AutorDto ────────────────────────────────────────────────────────────

    [Fact]
    public void AutorDto_CamposValidos_CreaCorrectamente()
    {
        var dto = new AutorDto(1, "Gabriel", "García Márquez", "Escritor colombiano");

        dto.Id.Should().Be(1);
        dto.Nombre.Should().Be("Gabriel");
        dto.Apellido.Should().Be("García Márquez");
        dto.Biografia.Should().Be("Escritor colombiano");
    }

    [Fact]
    public void AutorDto_BiografiaNula_EsValido()
    {
        var dto = new AutorDto(1, "Juan", "Pérez", null);

        dto.Biografia.Should().BeNull();
    }

    [Fact]
    public void PatchAutorDto_NombreRequerido_FallaValidacion()
    {
        var dto = new PatchAutorDto { Nombre = null!, Apellido = "López" };

        var results = Validate(dto);

        results.Should().Contain(r => r.MemberNames.Contains("Nombre"));
    }

    [Fact]
    public void PatchAutorDto_ApellidoRequerido_FallaValidacion()
    {
        var dto = new PatchAutorDto { Nombre = "Ana", Apellido = null! };

        var results = Validate(dto);

        results.Should().Contain(r => r.MemberNames.Contains("Apellido"));
    }

    [Fact]
    public void PatchAutorDto_TodosRequeridos_PasaValidacion()
    {
        var dto = new PatchAutorDto { Nombre = "Ana", Apellido = "Ruiz", Biografia = "Autora" };

        var results = Validate(dto);

        results.Should().BeEmpty();
    }

    // ── CategoriaDto ────────────────────────────────────────────────────────

    [Fact]
    public void CategoriaDto_CamposValidos_CreaCorrectamente()
    {
        var dto = new CategoriaDto(1, "Ficción", "Libros de ficción");

        dto.Id.Should().Be(1);
        dto.Nombre.Should().Be("Ficción");
        dto.Descripcion.Should().Be("Libros de ficción");
    }

    [Fact]
    public void PatchCategoriaDto_NombreRequerido_FallaValidacion()
    {
        var dto = new PatchCategoriaDto { Nombre = null! };

        var results = Validate(dto);

        results.Should().Contain(r => r.MemberNames.Contains("Nombre"));
    }

    [Fact]
    public void PatchCategoriaDto_NombreValido_PasaValidacion()
    {
        var dto = new PatchCategoriaDto { Nombre = "Ciencia", Descripcion = "Divulgación" };

        var results = Validate(dto);

        results.Should().BeEmpty();
    }

    // ── LibroDto ─────────────────────────────────────────────────────────────

    [Fact]
    public void LibroDto_CamposValidos_CreaCorrectamente()
    {
        var autores = new List<AutorDto> { new(1, "Gabriel", "García Márquez", null) };
        var dto = new LibroDto(1, "Cien años de soledad", null, "978-0-06-088328-7", 1967, 1, "Ficción", autores);

        dto.Id.Should().Be(1);
        dto.Titulo.Should().Be("Cien años de soledad");
        dto.Autores.Should().HaveCount(1);
    }

    [Fact]
    public void PatchLibroDto_TituloRequerido_FallaValidacion()
    {
        var dto = new PatchLibroDto { Titulo = null!, AnioPublicacion = 2020, CategoriaId = 1 };

        var results = Validate(dto);

        results.Should().Contain(r => r.MemberNames.Contains("Titulo"));
    }

    [Fact]
    public void CrearAutorDto_CamposValidos_CreaCorrectamente()
    {
        var dto = new CrearAutorDto("Carlos", "Fuentes", "Escritor mexicano");

        dto.Nombre.Should().Be("Carlos");
        dto.Apellido.Should().Be("Fuentes");
    }

    [Fact]
    public void CrearCategoriaDto_CamposValidos_CreaCorrectamente()
    {
        var dto = new CrearCategoriaDto("Filosofía", "Libros de filosofía");

        dto.Nombre.Should().Be("Filosofía");
        dto.Descripcion.Should().Be("Libros de filosofía");
    }

    [Fact]
    public void CrearLibroDto_CamposValidos_CreaCorrectamente()
    {
        var dto = new CrearLibroDto("Don Quijote", null, null, 1605, 1, new[] { 1 });

        dto.Titulo.Should().Be("Don Quijote");
        dto.CategoriaId.Should().Be(1);
        dto.AutoresIds.Should().Contain(1);
    }
}
