using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Autores.Commands;

public sealed record CreateAutorCommand(string Nombre, string Apellido, string? Biografia) : ICommand<AutorDto>;
