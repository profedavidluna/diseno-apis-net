using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public record EliminarLibroCommand(int Id) : IRequest<bool>;
