using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using BibliotecaApi.Exceptions;
using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Services;

public class AutorService(BibliotecaDbContext context) : IAutorService
{
    public async Task<IReadOnlyList<AutorResponse>> ListarAsync(CancellationToken ct)
    {
        return await context.Autores
            .AsNoTracking()
            .OrderBy(a => a.Nome)
            .Select(a => new AutorResponse(a.Id, a.Nome, a.Nacionalidade, a.DataNascimento, a.Livros.Count))
            .ToListAsync(ct);
    }

    public async Task<AutorResponse> ObterPorIdAsync(int id, CancellationToken ct)
    {
        return await context.Autores
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AutorResponse(a.Id, a.Nome, a.Nacionalidade, a.DataNascimento, a.Livros.Count))
            .FirstOrDefaultAsync(ct)
            ?? throw AutorNaoEncontrado(id);
    }

    public async Task<IReadOnlyList<LivroResponse>> ListarLivrosAsync(int autorId, CancellationToken ct)
    {
        if (!await context.Autores.AnyAsync(a => a.Id == autorId, ct))
            throw AutorNaoEncontrado(autorId);

        return await context.Livros
            .AsNoTracking()
            .Where(l => l.AutorId == autorId)
            .OrderBy(l => l.Titulo)
            .Select(l => new LivroResponse(l.Id, l.Titulo, l.Isbn, l.Genero, l.AnoPublicacao, l.QuantidadeEstoque, l.AutorId, l.Autor!.Nome))
            .ToListAsync(ct);
    }

    public async Task<AutorResponse> CriarAsync(AutorRequest request, CancellationToken ct)
    {
        ValidarDataNascimento(request.DataNascimento);

        var autor = new Autor
        {
            Nome = request.Nome.Trim(),
            Nacionalidade = request.Nacionalidade.Trim(),
            DataNascimento = request.DataNascimento
        };

        context.Autores.Add(autor);
        await context.SaveChangesAsync(ct);

        return ToResponse(autor, quantidadeLivros: 0);
    }

    public async Task<AutorResponse> AtualizarAsync(int id, AutorRequest request, CancellationToken ct)
    {
        ValidarDataNascimento(request.DataNascimento);

        var autor = await context.Autores.FindAsync([id], ct) ?? throw AutorNaoEncontrado(id);

        autor.Nome = request.Nome.Trim();
        autor.Nacionalidade = request.Nacionalidade.Trim();
        autor.DataNascimento = request.DataNascimento;

        await context.SaveChangesAsync(ct);

        var quantidadeLivros = await context.Livros.CountAsync(l => l.AutorId == id, ct);
        return ToResponse(autor, quantidadeLivros);
    }

    public async Task RemoverAsync(int id, CancellationToken ct)
    {
        var autor = await context.Autores.FindAsync([id], ct) ?? throw AutorNaoEncontrado(id);

        if (await context.Livros.AnyAsync(l => l.AutorId == id, ct))
            throw new ConflictException($"O autor {id} possui livros cadastrados e não pode ser removido.");

        context.Autores.Remove(autor);
        await context.SaveChangesAsync(ct);
    }

    private static void ValidarDataNascimento(DateOnly? dataNascimento)
    {
        if (dataNascimento > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessRuleException("A data de nascimento não pode estar no futuro.");
    }

    private static NotFoundException AutorNaoEncontrado(int id) =>
        new($"Autor com id {id} não encontrado.");

    private static AutorResponse ToResponse(Autor autor, int quantidadeLivros) =>
        new(autor.Id, autor.Nome, autor.Nacionalidade, autor.DataNascimento, quantidadeLivros);
}
