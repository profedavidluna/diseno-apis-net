namespace LibreriaAPI.Models.ReadModels;

public class CategoriaReadModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
