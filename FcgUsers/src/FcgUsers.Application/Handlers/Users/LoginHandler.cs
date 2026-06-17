using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Interfaces;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class LoginHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    ILogger<LoginHandler> logger)
: IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        LogLoginAttempt(logger, request.Email);

        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            LogLoginFailedUserNotFound(logger, request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Invalid credentials."));
        }

        if (user.IsInactive)
        {
            LogLoginFailedUserInactive(logger, request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Invalid credentials."));
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password.Hash))
        {
            LogLoginFailedInvalidPassword(logger, request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Invalid credentials."));
        }

        var token = tokenService.GenerateJwtToken(user.Email.Address, user.Role.ToString());

        LogLoginSuccess(logger, user.Id);

        return Result.Success(new LoginResponse(token));
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Login attempt initiated for email '{Email}'.")]
    private static partial void LogLoginAttempt(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Login failed: No user record found for email '{Email}'.")]
    private static partial void LogLoginFailedUserNotFound(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Login failed: Account for email '{Email}' is inactive.")]
    private static partial void LogLoginFailedUserInactive(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Login failed: Password mismatch for email '{Email}'.")]
    private static partial void LogLoginFailedInvalidPassword(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' successfully authenticated and logged in.")]
    private static partial void LogLoginSuccess(ILogger logger, Guid userId);
}
