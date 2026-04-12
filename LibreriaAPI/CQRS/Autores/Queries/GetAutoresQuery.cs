using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Autores.Queries;

public sealed record GetAutoresQuery() : IQuery<IEnumerable<AutorDto>>;
