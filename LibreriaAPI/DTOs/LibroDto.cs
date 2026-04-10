namespace LibreriaAPI.DTOs;

public record LibroDto(
    int Id,
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    string? CategoriaNombre,
    IEnumerable<AutorDto> Autores
);

public record CrearLibroDto(
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds
);

public record ActualizarLibroDto(
    string Titulo,
    string? Descripcion,
    string? ISBN,
    int AnioPublicacion,
    int CategoriaId,
    IEnumerable<int> AutoresIds
);
