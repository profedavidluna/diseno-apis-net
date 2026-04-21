using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public record EliminarAutorCommand(int Id) : IRequest<bool>;
