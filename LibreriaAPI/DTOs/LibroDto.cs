using System.ComponentModel.DataAnnotations;

namespace LibreriaAPI.DTOs;

public record LibroDto(
    int Id,
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    string? CategoriaNombre,
    IEnumerable<AutorDto> Autores
);

public record CrearLibroDto(
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds
);

public record ActualizarLibroDto(
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds
);

/// <summary>DTO mutable para actualizaciones parciales (JSON Patch) de un libro.</summary>
public class PatchLibroDto
{
    [Required]
    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? ISBN { get; set; }

    public int AnioPublicacion { get; set; }

    public int CategoriaId { get; set; }

    public IEnumerable<int>? AutoresIds { get; set; }
}
