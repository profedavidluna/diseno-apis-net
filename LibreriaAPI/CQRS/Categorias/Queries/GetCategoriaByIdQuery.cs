using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Categorias.Queries;

public sealed record GetCategoriaByIdQuery(int Id) : IQuery<CategoriaDto?>;
