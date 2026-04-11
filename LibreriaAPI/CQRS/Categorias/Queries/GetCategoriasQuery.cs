using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Categorias.Queries;

public sealed record GetCategoriasQuery() : IQuery<IEnumerable<CategoriaDto>>;
