namespace LibreriaAPI.DTOs;

public record CategoriaDto(int Id, string Nombre, string? Descripcion);

/// <summary>DTO de Categoría para la versión 2: incluye el campo NombreCompleto</summary>
public record CategoriaV2Dto(int Id, string Nombre, string? Descripcion, string NombreCompleto);

public record CrearCategoriaDto(string Nombre, string? Descripcion);

public record ActualizarCategoriaDto(string Nombre, string? Descripcion);
