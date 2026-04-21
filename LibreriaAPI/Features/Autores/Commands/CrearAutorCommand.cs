using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public record CrearAutorCommand(string Nombre, string Apellido, string? Biografia) : IRequest<AutorDto>;
