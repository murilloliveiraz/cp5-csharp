using BibliotecaApi.Dtos;

namespace BibliotecaApi.Services;

public interface IAutorService
{
    Task<IReadOnlyList<AutorResponse>> ListarAsync(CancellationToken ct);
    Task<AutorResponse> ObterPorIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<LivroResponse>> ListarLivrosAsync(int autorId, CancellationToken ct);
    Task<AutorResponse> CriarAsync(AutorRequest request, CancellationToken ct);
    Task<AutorResponse> AtualizarAsync(int id, AutorRequest request, CancellationToken ct);
    Task RemoverAsync(int id, CancellationToken ct);
}
