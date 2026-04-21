using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public record ActualizarAutorCommand(int Id, string Nombre, string Apellido, string? Biografia) : IRequest<bool>;
