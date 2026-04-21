using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public record EliminarCategoriaCommand(int Id) : IRequest<bool>;
