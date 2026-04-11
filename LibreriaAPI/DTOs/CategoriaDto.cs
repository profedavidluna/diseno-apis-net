namespace LibreriaAPI.DTOs;

public record CategoriaDto(int Id, string Nombre, string? Descripcion);

public record CrearCategoriaDto(string Nombre, string? Descripcion);

public record ActualizarCategoriaDto(string Nombre, string? Descripcion);
