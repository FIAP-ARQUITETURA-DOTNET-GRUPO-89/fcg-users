using MediatR;
using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using OperationResult;
using FcgUsers.Domain.Repositories;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class UpdatePasswordHandler(
    IUserRepository userRepository,
    ILogger<UpdatePasswordHandler> logger)
: IRequestHandler<UpdatePasswordCommand, Result<UpdatePasswordResponse>>
{
    public async Task<Result<UpdatePasswordResponse>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            return Result.Error<UpdatePasswordResponse>(new KeyNotFoundException($"Usuário com ID {request.Id} não foi encontrado."));
        }

        if (!string.Equals(user.Email.Address, request.RequestUserEmail, StringComparison.OrdinalIgnoreCase))
        {
            LogUnauthorizedPasswordUpdateAttempt(logger, request.Id, request.RequestUserId);
            return Result.Error<UpdatePasswordResponse>(new KeyNotFoundException($"Usuário com ID {request.Id} não foi encontrado."));
        }

        try
        {
            Password.ValidarTextoPuro(request.Password);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newPassword = Password.FromHash(passwordHash);

            user.ChangePassword(newPassword);

            userRepository.Update(user);
            await userRepository.SaveChangesAsync();

            LogPasswordUpdated(logger, user.Id, request.RequestUserId);

            return Result.Success(new UpdatePasswordResponse("Senha atualizada com sucesso!"));
        }
        catch (ArgumentException ex)
        {
            LogPasswordValidationFailed(logger, request.Id, ex.Message);
            return Result.Error<UpdatePasswordResponse>(ex);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "ALERTA: Usuário {RequestUserId} tentou alterar a senha do usuário {TargetUserId} que pertence a outra conta.")]
    private static partial void LogUnauthorizedPasswordUpdateAttempt(ILogger logger, Guid targetUserId, Guid requestUserId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Senha do usuário {TargetUserId} atualizada com sucesso pelo usuário {RequestUserId}.")]
    private static partial void LogPasswordUpdated(ILogger logger, Guid targetUserId, Guid requestUserId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro de validação de domínio ao atualizar senha do usuário {TargetUserId}: {Reason}")]
    private static partial void LogPasswordValidationFailed(ILogger logger, Guid targetUserId, string reason);
}
