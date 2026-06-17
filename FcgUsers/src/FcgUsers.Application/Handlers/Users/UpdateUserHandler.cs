using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class UpdateUserHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserHandler> logger)
: IRequestHandler<UpdateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        LogUpdateUserStarted(logger, request.Id);

        var user = await userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            LogUserNotFound(logger, request.Id);
            return Result.Error<UserResponse>(new ArgumentException("User not found."));
        }

        try
        {
            user.UpdateProfile(request.Name, request.BirthDate);
        }
        catch (ArgumentException ex)
        {
            LogUpdateUserValidationFailed(logger, request.Id, ex.Message);
            return Result.Error<UserResponse>(ex);
        }

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        LogUpdateUserSuccess(logger, user.Id);

        var response = user.ToResponse();
        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Initiating profile update for user ID '{userId}'")]
    private static partial void LogUpdateUserStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Profile update failed. User with ID '{userId}' was not found")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Domain validation failed for user ID '{userId}': {reason}")]
    private static partial void LogUpdateUserValidationFailed(ILogger logger, Guid userId, string reason);

    [LoggerMessage(Level = LogLevel.Information, Message = "Profile for user '{userId}' successfully updated")]
    private static partial void LogUpdateUserSuccess(ILogger logger, Guid userId);
}
