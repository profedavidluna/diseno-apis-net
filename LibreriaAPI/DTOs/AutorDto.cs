namespace LibreriaAPI.DTOs;

public record AutorDto(int Id, string Nombre, string Apellido, string? Biografia);

public record AutorV2Dto(int Id, string Nombre, string Apellido, string? Biografia, string NombreCompleto);

public record CrearAutorDto(string Nombre, string Apellido, string? Biografia);

public record ActualizarAutorDto(string Nombre, string Apellido, string? Biografia);
