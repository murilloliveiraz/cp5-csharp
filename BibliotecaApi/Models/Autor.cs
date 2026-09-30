namespace BibliotecaApi.Models;

public class Autor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Nacionalidade { get; set; } = string.Empty;
    public DateOnly? DataNascimento { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Livro> Livros { get; set; } = new List<Livro>();
}
