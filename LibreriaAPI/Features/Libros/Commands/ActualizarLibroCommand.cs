using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public record ActualizarLibroCommand(
    int Id,
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds) : IRequest<ActualizarLibroResult>;

public record ActualizarLibroResult(bool Encontrado, string? Error);
