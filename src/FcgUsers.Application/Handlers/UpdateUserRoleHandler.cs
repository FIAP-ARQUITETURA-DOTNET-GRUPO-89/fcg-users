using MediatR;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Enums;
using Microsoft.Extensions.Logging;
using OperationResult;
using FcgUsers.Domain.Repositories;
using FcgUsers.Application.Commands;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class UpdateUserRoleHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserRoleHandler> logger)
: IRequestHandler<UpdateUserRoleCommand, Result<UpdateUserRoleResponse>>
{
    public async Task<Result<UpdateUserRoleResponse>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        LogUpdateRoleStarted(logger, request.Id);

        if (!Enum.TryParse<UserRole>(request.RoleName, true, out var newRole))
        {
            LogInvalidRoleAttempt(logger, request.RoleName, request.Id);
            return Result.Error<UpdateUserRoleResponse>(new ArgumentException("Nome de nível de acesso inválido."));
        }

        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            LogUserNotFound(logger, request.Id);
            return Result.Error<UpdateUserRoleResponse>(new KeyNotFoundException("Usuário não encontrado."));
        }

        if (user.Role == UserRole.Admin && newRole == UserRole.User)
        {
            LogAdminDemotionBlocked(logger, user.Id);
            return Result.Error<UpdateUserRoleResponse>(new InvalidOperationException("Não é permitido rebaixar um administrador para usuário padrão por este fluxo."));
        }

        user.ChangeRole(newRole);
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        LogRoleUpdatedSuccess(logger, user.Id, user.Role.ToString());

        var response = user.ToUpdateRoleResponse("Nível de acesso atualizado com sucesso!");

        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando a atualização de nível de acesso para o usuário ID '{userId}'")]
    private static partial void LogUpdateRoleStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Usuário '{userId}' não foi encontrado para atualização de nível de acesso.")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Tentativa de atribuir um nível de acesso inválido '{roleName}' ao usuário '{userId}'.")]
    private static partial void LogInvalidRoleAttempt(ILogger logger, string roleName, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tentativa bloqueada de rebaixar o Admin '{userId}' para Usuário padrão.")]
    private static partial void LogAdminDemotionBlocked(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Nível de acesso do usuário '{userId}' atualizado para '{role}' com sucesso.")]
    private static partial void LogRoleUpdatedSuccess(ILogger logger, Guid userId, string role);
}
