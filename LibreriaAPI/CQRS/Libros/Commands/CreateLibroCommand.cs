using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Libros.Commands;

public sealed record CreateLibroCommand(
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds
) : ICommand<CreateLibroResult>;

/// <summary>Resultado del comando CreateLibro, indicando éxito o el tipo de error.</summary>
public sealed record CreateLibroResult(LibroDto? Libro, bool CategoriaNoEncontrada, bool AutoresInvalidos);
