using Microsoft.AspNetCore.Mvc;
using FluentValidation; // Importante

namespace FcgUsers.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex) // Adicione este bloco!
        {
            logger.LogWarning("Erro de validação: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            // Retorna os detalhes dos erros de validação
            var problem = new ValidationProblemDetails(
                ex.Errors.GroupBy(e => e.PropertyName)
                          .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            )
            { Status = StatusCodes.Status400BadRequest };

            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Operação inválida: {Message}", ex.Message);
            await WriteProblemDetails(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Erro interno.");
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, int statusCode, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails { Status = statusCode, Detail = detail };
        await context.Response.WriteAsJsonAsync(problem);
    }
}
