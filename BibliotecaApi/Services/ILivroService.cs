using BibliotecaApi.Dtos;

namespace BibliotecaApi.Services;

public interface ILivroService
{
    Task<IReadOnlyList<LivroResponse>> ListarAsync(string? genero, CancellationToken ct);
    Task<LivroResponse> ObterPorIdAsync(int id, CancellationToken ct);
    Task<LivroResponse> CriarAsync(LivroRequest request, CancellationToken ct);
    Task<LivroResponse> AtualizarAsync(int id, LivroRequest request, CancellationToken ct);
    Task RemoverAsync(int id, CancellationToken ct);
}
