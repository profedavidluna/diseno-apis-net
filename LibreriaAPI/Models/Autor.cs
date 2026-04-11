namespace LibreriaAPI.Models;

public class Autor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Biografia { get; set; }

    public ICollection<LibroAutor> LibroAutores { get; set; } = new List<LibroAutor>();
}
