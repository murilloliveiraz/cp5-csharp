using BibliotecaApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Middleware;

/// <summary>
/// Converte exceções em respostas ProblemDetails (RFC 7807) com o status code adequado.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            BusinessRuleException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado ao processar {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
