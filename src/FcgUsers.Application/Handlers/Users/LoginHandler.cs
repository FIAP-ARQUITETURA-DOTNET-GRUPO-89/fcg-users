using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Interfaces;
using FcgUsers.Application.Responses.Users;
using Microsoft.Extensions.Logging;
using OperationResult;
using FcgUsers.Domain.Repositories;

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

        var user = await userRepository.GetByEmailAsync(request.UserEmail.Address);

        if (user is null)
        {
            LogLoginFailedUserNotFound(logger, request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Credenciais inválidas."));
        }

        if (user.IsInactive)
        {
            LogLoginFailedUserInactive(logger, request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Credenciais inválidas."));
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password.Hash))
        {
            LogLoginFailedInvalidPassword(logger, request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Credenciais inválidas."));
        }

        var token = tokenService.GenerateJwtToken(user.Email.Address, user.Role.ToString());

        LogLoginSuccess(logger, user.Id);

        return Result.Success(new LoginResponse(token));
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Tentativa de login iniciada para o e-mail '{Email}'.")]
    private static partial void LogLoginAttempt(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Falha no login: Nenhum registro de usuário encontrado para o e-mail '{Email}'.")]
    private static partial void LogLoginFailedUserNotFound(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Falha no login: A conta para o e-mail '{Email}' está inativa.")]
    private static partial void LogLoginFailedUserInactive(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Falha no login: Senha incorreta para o e-mail '{Email}'.")]
    private static partial void LogLoginFailedInvalidPassword(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "Usuário com ID '{UserId}' autenticado e logado com sucesso.")]
    private static partial void LogLoginSuccess(ILogger logger, Guid userId);
}
