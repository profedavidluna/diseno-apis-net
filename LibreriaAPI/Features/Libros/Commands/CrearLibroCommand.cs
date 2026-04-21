using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public record CrearLibroCommand(
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds) : IRequest<CrearLibroResult>;

public record CrearLibroResult(LibroDto? Libro, string? Error);
