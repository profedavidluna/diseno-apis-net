using System.ComponentModel.DataAnnotations;

namespace LibreriaAPI.DTOs;

public record CategoriaDto(int Id, string Nombre, string? Descripcion);

public record CrearCategoriaDto(string Nombre, string? Descripcion);

public record ActualizarCategoriaDto(string Nombre, string? Descripcion);

/// <summary>DTO mutable para actualizaciones parciales (JSON Patch) de una categoría.</summary>
public class PatchCategoriaDto
{
    [Required]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
}
