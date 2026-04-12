using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Libros.Queries;

public sealed record GetLibrosQuery() : IQuery<IEnumerable<LibroDto>>;
