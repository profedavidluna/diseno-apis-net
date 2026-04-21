using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Queries;

public record GetCategoriasQuery : IRequest<IEnumerable<CategoriaDto>>;
