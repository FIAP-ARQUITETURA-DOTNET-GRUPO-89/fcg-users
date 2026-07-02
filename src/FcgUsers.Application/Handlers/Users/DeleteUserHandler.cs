using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using Microsoft.Extensions.Logging;
using OperationResult;
using FcgUsers.Domain.Repositories;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class DeleteUserHandler(
    IUserRepository userRepository,
    ILogger<DeleteUserHandler> logger)
: IRequestHandler<DeleteUserCommand, Result<DeleteUserResponse>>
{
    public async Task<Result<DeleteUserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        LogDeleteUserStarted(logger, request.Id);

        var user = await userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            LogUserNotFound(logger, request.Id);
            return Result.Error<DeleteUserResponse>(new ArgumentException("Usuário não encontrado."));
        }

        user.Deactivate();

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        LogUserDeletedSuccess(logger, user.Id);

        var message = $"O usuário {user.Name} foi desativado com sucesso.";

        var response = user.ToDeleteResponse(message);

        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando a desativação do usuário com ID '{userId}'")]
    private static partial void LogDeleteUserStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tentativa de desativação falhou. Usuário com ID '{userId}' não foi encontrado")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Usuário '{userId}' desativado com sucesso")]
    private static partial void LogUserDeletedSuccess(ILogger logger, Guid userId);
}
