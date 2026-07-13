using FcgUsers.Application.Commands;
using FcgUsers.Application.Responses.Users;
using FgcGames.Api.Filters;
using MediatR;

namespace FcgUsers.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/login", Login)
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<LoginCommand>>()
            .WithSummary("Realiza a autenticação do usuário")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Login(LoginCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Unauthorized();
    }
}
