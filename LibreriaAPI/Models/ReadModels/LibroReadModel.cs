namespace LibreriaAPI.Models.ReadModels;

public class LibroReadModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ISBN { get; set; }
    public int AnioPublicacion { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    /// <summary>JSON-serialized list of AutorDto</summary>
    public string AutoresJson { get; set; } = "[]";
}
