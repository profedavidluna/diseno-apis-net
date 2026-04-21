namespace LibreriaAPI.Models.ReadModels;

/// <summary>
/// Tabla de índice en la BD de lectura que permite localizar eficientemente
/// los libros asociados a un autor sin cargar todos los registros.
/// </summary>
public class LibroAutorReadModel
{
    public int LibroId { get; set; }
    public int AutorId { get; set; }
}
