using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Libros.Queries;

public sealed record GetLibroByIdQuery(int Id) : IQuery<LibroDto?>;
