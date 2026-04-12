using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Autores.Commands;

public sealed record UpdateAutorCommand(int Id, string Nombre, string Apellido, string? Biografia) : ICommand<AutorDto?>;
