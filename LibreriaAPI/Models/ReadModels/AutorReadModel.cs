namespace LibreriaAPI.Models.ReadModels;

public class AutorReadModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Biografia { get; set; }
}
