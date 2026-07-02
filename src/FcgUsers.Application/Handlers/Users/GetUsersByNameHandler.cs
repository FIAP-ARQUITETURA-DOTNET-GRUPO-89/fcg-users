using MediatR;
using FcgUsers.Application.Queries.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class GetUsersByNameHandler(
    IUserRepository userRepository,
    ILogger<GetUsersByNameHandler> logger)
: IRequestHandler<GetUsersByNameQuery, Result<PagedResponse<UserResponse>>>
{
    public async Task<Result<PagedResponse<UserResponse>>> Handle(GetUsersByNameQuery request, CancellationToken cancellationToken)
    {
        LogFetchUsersByNameStarted(logger, request.Name, request.Page, request.PageSize);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            var emptyResponse = new PagedResponse<UserResponse>(
                Enumerable.Empty<UserResponse>(),
                request.Page,
                request.PageSize,
                0
            );
            return Result.Success(emptyResponse);
        }

        var (users, totalCount) = await userRepository.GetByNamePagedAsync(
            request.Name,
            request.Page,
            request.PageSize,
            cancellationToken
        );

        var userResponses = users.ToResponseList().ToList();

        var pagedResponse = new PagedResponse<UserResponse>(
            userResponses,
            request.Page,
            request.PageSize,
            totalCount
        );

        LogFetchUsersByNameSuccess(logger, request.Name, users.Count, totalCount);

        return Result.Success(pagedResponse);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Buscando usuários com nome contendo '{Name}' - Página: {Page}, Tamanho: {PageSize}.")]
    private static partial void LogFetchUsersByNameStarted(ILogger logger, string name, int page, int pageSize);

    [LoggerMessage(Level = LogLevel.Information, Message = "Busca por '{Name}' concluída. Encontrados {Count} usuários compatíveis na página atual de um total de {TotalCount}.")]
    private static partial void LogFetchUsersByNameSuccess(ILogger logger, string name, int count, int totalCount);
}
