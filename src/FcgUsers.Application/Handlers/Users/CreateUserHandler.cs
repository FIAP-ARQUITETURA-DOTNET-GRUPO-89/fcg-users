using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class CreateUserHandler(
    IUserRepository userRepository,
    ILogger<CreateUserHandler> logger)
: IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        LogUserCreationStarted(logger, request.Email);

        var alreadyExists = await userRepository.ExistsByEmailAsync(request.Email);
        if (alreadyExists)
        {
            LogUserAlreadyExists(logger, request.Email);
            return Result.Error<CreateUserResponse>(new ArgumentException("Já existe um usuário com esse email"));
        }

        var user = request.ToEntity();
        userRepository.Add(user);
        await userRepository.SaveChangesAsync();

        LogUserCreated(logger, user.Id, user.Email.Address);

        return Result.Success(user.ToCreateUserResponse());
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando a criação do usuário com e-mail '{email}'")]
    private static partial void LogUserCreationStarted(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tentativa de cadastro falhou. Já existe um usuário com o e-mail '{email}'")]
    private static partial void LogUserAlreadyExists(ILogger logger, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "Usuário '{userId}' criado com sucesso com o e-mail '{email}'")]
    private static partial void LogUserCreated(ILogger logger, Guid userId, string email);
}
