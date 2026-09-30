using Asp.Versioning;
using BibliotecaApi.Dtos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/livros")]
[Produces("application/json")]
public class LivrosController(ILivroService livroService) : ControllerBase
{
    /// <summary>Lista os livros, com filtro opcional por gênero.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<LivroResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LivroResponse>>> Listar([FromQuery] string? genero, CancellationToken ct)
    {
        return Ok(await livroService.ListarAsync(genero, ct));
    }

    /// <summary>Retorna um livro pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<LivroResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponse>> ObterPorId(int id, CancellationToken ct)
    {
        return Ok(await livroService.ObterPorIdAsync(id, ct));
    }

    /// <summary>Cadastra um novo livro.</summary>
    [HttpPost]
    [ProducesResponseType<LivroResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LivroResponse>> Criar([FromBody] LivroRequest request, CancellationToken ct)
    {
        var livro = await livroService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = livro.Id, version = "1" }, livro);
    }

    /// <summary>Atualiza todos os dados de um livro.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<LivroResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LivroResponse>> Atualizar(int id, [FromBody] LivroRequest request, CancellationToken ct)
    {
        return Ok(await livroService.AtualizarAsync(id, request, ct));
    }

    /// <summary>Remove um livro.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await livroService.RemoverAsync(id, ct);
        return NoContent();
    }
}
