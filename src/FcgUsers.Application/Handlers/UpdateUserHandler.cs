using MediatR;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using Microsoft.Extensions.Logging;
using OperationResult;
using FcgUsers.Domain.Repositories;
using FcgUsers.Application.Commands;

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
            return Result.Error<UserResponse>(new KeyNotFoundException("Usuário não encontrado."));
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

    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando a atualização de perfil do usuário com ID '{userId}'")]
    private static partial void LogUpdateUserStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Atualização de perfil falhou. Usuário com ID '{userId}' não foi encontrado")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Validação de domínio falhou para o usuário ID '{userId}': {reason}")]
    private static partial void LogUpdateUserValidationFailed(ILogger logger, Guid userId, string reason);

    [LoggerMessage(Level = LogLevel.Information, Message = "Perfil do usuário '{userId}' atualizado com sucesso")]
    private static partial void LogUpdateUserSuccess(ILogger logger, Guid userId);
}
