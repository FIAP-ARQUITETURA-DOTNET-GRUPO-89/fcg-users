using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

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
            return Result.Error<DeleteUserResponse>(new ArgumentException("User not found."));
        }

        user.Deactivate();

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        LogUserDeletedSuccess(logger, user.Id);

        var message = $"The user {user.Name} has been successfully deactivated.";
        var response = user.ToDeleteResponse(message);

        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Initiating deactivation for user ID '{userId}'")]
    private static partial void LogDeleteUserStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Deactivation failed. User with ID '{userId}' was not found")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User '{userId}' successfully deactivated")]
    private static partial void LogUserDeletedSuccess(ILogger logger, Guid userId);
}
