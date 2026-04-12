using LibreriaAPI.CQRS.Abstractions;

namespace LibreriaAPI.CQRS.Libros.Commands;

public sealed record UpdateLibroCommand(
    int Id,
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds
) : ICommand<UpdateLibroResult>;

/// <summary>Resultado del comando UpdateLibro, indicando éxito o el tipo de error.</summary>
public sealed record UpdateLibroResult(bool LibroNoEncontrado, bool CategoriaNoEncontrada, bool AutoresInvalidos);
