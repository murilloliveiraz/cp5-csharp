using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Dtos;

public record LivroRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    string Titulo,

    [Required(ErrorMessage = "O ISBN é obrigatório.")]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "O ISBN deve conter exatamente 13 dígitos numéricos.")]
    string Isbn,

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [StringLength(60, ErrorMessage = "O gênero deve ter no máximo 60 caracteres.")]
    string Genero,

    [Range(1450, 2100, ErrorMessage = "O ano de publicação deve estar entre 1450 e 2100.")]
    int AnoPublicacao,

    [Range(0, 10000, ErrorMessage = "A quantidade em estoque deve estar entre 0 e 10000.")]
    int QuantidadeEstoque,

    [Range(1, int.MaxValue, ErrorMessage = "O AutorId deve ser um identificador válido.")]
    int AutorId);

public record LivroResponse(
    int Id,
    string Titulo,
    string Isbn,
    string Genero,
    int AnoPublicacao,
    int QuantidadeEstoque,
    int AutorId,
    string NomeAutor);
