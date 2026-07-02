using MediatR;
using FcgUsers.Application.Queries.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class GetUserByIdHandler(
    IUserRepository userRepository,
    ILogger<GetUserByIdHandler> logger)
: IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        LogFetchUserByIdStarted(logger, request.Id);

        var user = await userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            LogUserNotFound(logger, request.Id);
            return Result.Error<UserResponse>(new KeyNotFoundException($"Usuário com o ID {request.Id} não foi encontrado."));
        }

        LogFetchUserByIdSuccess(logger, user.Id);

        var response = user.ToResponse();
        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Buscando detalhes do usuário com ID '{userId}'.")]
    private static partial void LogFetchUserByIdStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Busca falhou. Usuário com ID '{userId}' não foi encontrado.")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Detalhes do usuário ID '{userId}' recuperados com sucesso.")]
    private static partial void LogFetchUserByIdSuccess(ILogger logger, Guid userId);
}
