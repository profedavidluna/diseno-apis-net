using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Queries;

public record GetCategoriaByIdQuery(int Id) : IRequest<CategoriaDto?>;
