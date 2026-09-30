namespace BibliotecaApi.Models;

public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public int QuantidadeEstoque { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public int AutorId { get; set; }
    public Autor? Autor { get; set; }
}
