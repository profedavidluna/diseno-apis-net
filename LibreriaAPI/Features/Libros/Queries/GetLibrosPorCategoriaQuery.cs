using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Libros.Queries;

public record GetLibrosPorCategoriaQuery(int CategoriaId) : IRequest<IEnumerable<LibroDto>>;
