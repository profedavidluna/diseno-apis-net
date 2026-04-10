namespace LibreriaAPI.Models;

public class Libro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ISBN { get; set; }
    public int AnioPublicacion { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public ICollection<LibroAutor> LibroAutores { get; set; } = new List<LibroAutor>();
}
