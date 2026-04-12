using System.ComponentModel.DataAnnotations;

namespace LibreriaAPI.DTOs;

public record AutorDto(int Id, string Nombre, string Apellido, string? Biografia);

public record CrearAutorDto(string Nombre, string Apellido, string? Biografia);

public record ActualizarAutorDto(string Nombre, string Apellido, string? Biografia);

/// <summary>DTO mutable para actualizaciones parciales (JSON Patch) de un autor.</summary>
public class PatchAutorDto
{
    [Required]
    public string Nombre { get; set; } = null!;

    [Required]
    public string Apellido { get; set; } = null!;

    public string? Biografia { get; set; }
}
