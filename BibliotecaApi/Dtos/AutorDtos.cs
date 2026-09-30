using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Dtos;

public record AutorRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "A nacionalidade é obrigatória.")]
    [StringLength(80, ErrorMessage = "A nacionalidade deve ter no máximo 80 caracteres.")]
    string Nacionalidade,

    DateOnly? DataNascimento);

public record AutorResponse(
    int Id,
    string Nome,
    string Nacionalidade,
    DateOnly? DataNascimento,
    int QuantidadeLivros);
