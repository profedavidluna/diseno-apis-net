using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Libros.Queries;

public record GetLibrosQuery : IRequest<IEnumerable<LibroDto>>;
