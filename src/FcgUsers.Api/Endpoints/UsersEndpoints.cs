using FcgUsers.Application.Commands;
using FcgUsers.Application.Queries;
using FcgUsers.Application.Responses;
using FcgUsers.Application.Responses.Users;
using FgcGames.Api.Filters;
using MediatR;

namespace FcgUsers.Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Usuários");

        group.MapPost("/", CreateUser)
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
            .WithSummary("Cria um novo usuário")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/admin", CreateAdminUser)
            .RequireAuthorization("AdminPolicy")
            .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
            .WithSummary("Cria um novo usuário administrador")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetUserById)
            .RequireAuthorization("AdminPolicy")
            .WithSummary("Obtém um usuário pelo ID")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", GetAllUsers)
            .RequireAuthorization("AdminPolicy")
            .AddEndpointFilter<ValidationFilter<GetAllUsersQuery>>()
            .WithSummary("Lista todos os usuários")
            .Produces<PagedResponse<UserResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/busca", GetUsersByName)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<GetUsersByNameQuery>>()
            .WithSummary("Busca usuários por nome")
            .Produces<PagedResponse<UserResponse>>(StatusCodes.Status200OK);

        group.MapPut("/{id:guid}", UpdateUser)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>()
            .WithSummary("Atualiza um usuário")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:guid}/role", UpdateUserRole)
            .RequireAuthorization("AdminPolicy")
            .AddEndpointFilter<ValidationFilter<UpdateUserRoleCommand>>()
            .WithSummary("Atualiza a role de um usuário")
            .Produces<UpdateUserRoleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPatch("/{id:guid}/password", UpdatePassword)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdatePasswordCommand>>()
            .WithSummary("Atualiza a senha de um usuário")
            .Produces<UpdatePasswordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteUser)
            .RequireAuthorization("AdminPolicy")
            .AddEndpointFilter<ValidationFilter<DeleteUserCommand>>()
            .WithSummary("Remove um usuário")
            .Produces<DeleteUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateUser(CreateUserCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command with { Role = FcgUsers.Domain.Enums.UserRole.Customer.ToString() });
        return result.IsSuccess ? Results.Created($"/api/users/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Exception);
    }

    private static async Task<IResult> CreateAdminUser(CreateUserCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command with { Role = FcgUsers.Domain.Enums.UserRole.Admin.ToString() });
        return result.IsSuccess ? Results.Created($"/api/users/{result.Value!.Id}", result.Value) : Results.BadRequest(result.Exception);
    }

    private static async Task<IResult> GetUserById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        return result.IsSuccess ? Results.Ok(result.Value) : CreateNotFoundProblem(id);
    }

    private static async Task<IResult> GetAllUsers([AsParameters] GetAllUsersQuery query, IMediator mediator)
    {
        var result = await mediator.Send(query);
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetUsersByName([AsParameters] GetUsersByNameQuery query, IMediator mediator)
    {
        var result = await mediator.Send(query);
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateUser(Guid id, UpdateUserCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.IsSuccess ? Results.Ok(result.Value) : CreateNotFoundProblem(id);
    }

    private static async Task<IResult> UpdateUserRole(Guid id, UpdateUserRoleCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command with { Id = id });

        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { message = result.Exception?.Message });
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdatePassword(Guid id, UpdatePasswordCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.IsSuccess ? Results.Ok(result.Value) : CreateNotFoundProblem(id);
    }

    private static async Task<IResult> DeleteUser(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteUserCommand(id));
        return result.IsSuccess ? Results.Ok(result.Value) : CreateNotFoundProblem(id);
    }

    private static IResult CreateNotFoundProblem(Guid id)
    {
        return Results.Problem(
            detail: $"O usuário com o ID {id} não foi encontrado no sistema.",
            statusCode: StatusCodes.Status404NotFound,
            title: "Recurso não encontrado",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        );
    }
}
