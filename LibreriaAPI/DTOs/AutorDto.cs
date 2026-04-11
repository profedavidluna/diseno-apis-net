namespace LibreriaAPI.DTOs;

public record AutorDto(int Id, string Nombre, string Apellido, string? Biografia);

public record CrearAutorDto(string Nombre, string Apellido, string? Biografia);

public record ActualizarAutorDto(string Nombre, string Apellido, string? Biografia);
