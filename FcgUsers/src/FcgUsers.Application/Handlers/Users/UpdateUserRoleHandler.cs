using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class UpdateUserRoleHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserRoleHandler> logger)
: IRequestHandler<UpdateUserRoleCommand, Result<UpdateUserRoleResponse>>
{
    public async Task<Result<UpdateUserRoleResponse>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        LogUpdateRoleStarted(logger, request.Id);

        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            LogUserNotFound(logger, request.Id);
            return Result.Error<UpdateUserRoleResponse>(new ArgumentException("User not found."));
        }

        if (!Enum.TryParse<UserRole>(request.RoleName, true, out var newRole))
        {
            LogInvalidRoleAttempt(logger, request.RoleName, request.Id);
            return Result.Error<UpdateUserRoleResponse>(new ArgumentException("Invalid role name."));
        }

        if (user.Role == UserRole.Admin && newRole == UserRole.User)
        {
            LogAdminDemotionBlocked(logger, user.Id);
            return Result.Error<UpdateUserRoleResponse>(new InvalidOperationException("Demoting an administrator to a standard user is not allowed through this flow."));
        }

        user.ChangeRole(newRole);
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        LogRoleUpdatedSuccess(logger, user.Id, user.Role.ToString());

        var response = user.ToUpdateRoleResponse("Role updated successfully!");

        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Initiating role update for user ID '{userId}'")]
    private static partial void LogUpdateRoleStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "User '{userId}' not found for role update.")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Attempted to assign an invalid role name '{roleName}' to user '{userId}'.")]
    private static partial void LogInvalidRoleAttempt(ILogger logger, string roleName, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Blocked attempt to demote Admin '{userId}' to standard User.")]
    private static partial void LogAdminDemotionBlocked(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Role for user '{userId}' updated to '{role}' successfully.")]
    private static partial void LogRoleUpdatedSuccess(ILogger logger, Guid userId, string role);
}
