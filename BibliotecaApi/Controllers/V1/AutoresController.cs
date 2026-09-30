using Asp.Versioning;
using BibliotecaApi.Dtos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/autores")]
[Produces("application/json")]
public class AutoresController(IAutorService autorService) : ControllerBase
{
    /// <summary>Lista todos os autores cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AutorResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AutorResponse>>> Listar(CancellationToken ct)
    {
        return Ok(await autorService.ListarAsync(ct));
    }

    /// <summary>Retorna um autor pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<AutorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponse>> ObterPorId(int id, CancellationToken ct)
    {
        return Ok(await autorService.ObterPorIdAsync(id, ct));
    }

    /// <summary>Lista os livros de um autor.</summary>
    [HttpGet("{id:int}/livros")]
    [ProducesResponseType<IReadOnlyList<LivroResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<LivroResponse>>> ListarLivros(int id, CancellationToken ct)
    {
        return Ok(await autorService.ListarLivrosAsync(id, ct));
    }

    /// <summary>Cadastra um novo autor.</summary>
    [HttpPost]
    [ProducesResponseType<AutorResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutorResponse>> Criar([FromBody] AutorRequest request, CancellationToken ct)
    {
        var autor = await autorService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = autor.Id, version = "1" }, autor);
    }

    /// <summary>Atualiza todos os dados de um autor.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<AutorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponse>> Atualizar(int id, [FromBody] AutorRequest request, CancellationToken ct)
    {
        return Ok(await autorService.AtualizarAsync(id, request, ct));
    }

    /// <summary>Remove um autor (somente se não possuir livros).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await autorService.RemoverAsync(id, ct);
        return NoContent();
    }
}
