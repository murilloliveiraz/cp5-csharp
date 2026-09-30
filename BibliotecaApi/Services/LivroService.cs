using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using BibliotecaApi.Exceptions;
using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Services;

public class LivroService(BibliotecaDbContext context) : ILivroService
{
    public async Task<IReadOnlyList<LivroResponse>> ListarAsync(string? genero, CancellationToken ct)
    {
        var query = context.Livros.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(genero))
        {
            var generoNormalizado = genero.Trim().ToLower();
            query = query.Where(l => l.Genero.ToLower() == generoNormalizado);
        }

        return await query
            .OrderBy(l => l.Titulo)
            .Select(l => new LivroResponse(l.Id, l.Titulo, l.Isbn, l.Genero, l.AnoPublicacao, l.QuantidadeEstoque, l.AutorId, l.Autor!.Nome))
            .ToListAsync(ct);
    }

    public async Task<LivroResponse> ObterPorIdAsync(int id, CancellationToken ct)
    {
        return await context.Livros
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LivroResponse(l.Id, l.Titulo, l.Isbn, l.Genero, l.AnoPublicacao, l.QuantidadeEstoque, l.AutorId, l.Autor!.Nome))
            .FirstOrDefaultAsync(ct)
            ?? throw LivroNaoEncontrado(id);
    }

    public async Task<LivroResponse> CriarAsync(LivroRequest request, CancellationToken ct)
    {
        var autor = await BuscarAutorAsync(request.AutorId, ct);
        await GarantirIsbnDisponivelAsync(request.Isbn, livroIdAtual: null, ct);

        var livro = new Livro { Autor = autor };
        AplicarDados(livro, request);

        context.Livros.Add(livro);
        await context.SaveChangesAsync(ct);

        return ToResponse(livro, autor);
    }

    public async Task<LivroResponse> AtualizarAsync(int id, LivroRequest request, CancellationToken ct)
    {
        var livro = await context.Livros.FindAsync([id], ct) ?? throw LivroNaoEncontrado(id);
        var autor = await BuscarAutorAsync(request.AutorId, ct);
        await GarantirIsbnDisponivelAsync(request.Isbn, livroIdAtual: id, ct);

        AplicarDados(livro, request);
        livro.Autor = autor;

        await context.SaveChangesAsync(ct);

        return ToResponse(livro, autor);
    }

    public async Task RemoverAsync(int id, CancellationToken ct)
    {
        var livro = await context.Livros.FindAsync([id], ct) ?? throw LivroNaoEncontrado(id);

        context.Livros.Remove(livro);
        await context.SaveChangesAsync(ct);
    }

    private async Task<Autor> BuscarAutorAsync(int autorId, CancellationToken ct) =>
        await context.Autores.FindAsync([autorId], ct)
        ?? throw new BusinessRuleException($"Não existe autor com id {autorId}. Informe um AutorId válido.");

    private async Task GarantirIsbnDisponivelAsync(string isbn, int? livroIdAtual, CancellationToken ct)
    {
        var isbnEmUso = await context.Livros.AnyAsync(l => l.Isbn == isbn && l.Id != livroIdAtual, ct);
        if (isbnEmUso)
            throw new ConflictException($"Já existe um livro cadastrado com o ISBN {isbn}.");
    }

    private static void AplicarDados(Livro livro, LivroRequest request)
    {
        livro.Titulo = request.Titulo.Trim();
        livro.Isbn = request.Isbn;
        livro.Genero = request.Genero.Trim();
        livro.AnoPublicacao = request.AnoPublicacao;
        livro.QuantidadeEstoque = request.QuantidadeEstoque;
        livro.AutorId = request.AutorId;
    }

    private static NotFoundException LivroNaoEncontrado(int id) =>
        new($"Livro com id {id} não encontrado.");

    private static LivroResponse ToResponse(Livro livro, Autor autor) =>
        new(livro.Id, livro.Titulo, livro.Isbn, livro.Genero, livro.AnoPublicacao, livro.QuantidadeEstoque, autor.Id, autor.Nome);
}
