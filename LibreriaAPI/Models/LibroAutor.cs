namespace LibreriaAPI.Models;

public class LibroAutor
{
    public int LibroId { get; set; }
    public Libro? Libro { get; set; }

    public int AutorId { get; set; }
    public Autor? Autor { get; set; }
}
