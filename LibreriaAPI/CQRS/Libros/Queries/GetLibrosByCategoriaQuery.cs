using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Libros.Queries;

public sealed record GetLibrosByCategoriaQuery(int CategoriaId) : IQuery<IEnumerable<LibroDto>>;
