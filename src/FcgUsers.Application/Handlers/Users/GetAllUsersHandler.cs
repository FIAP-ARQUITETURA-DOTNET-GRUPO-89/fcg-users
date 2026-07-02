using MediatR;
using FcgUsers.Application.Queries.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Users;

public sealed partial class GetAllUsersHandler(
    IUserRepository userRepository,
    ILogger<GetAllUsersHandler> logger)
: IRequestHandler<GetAllUsersQuery, Result<PagedResponse<UserResponse>>>
{
    public async Task<Result<PagedResponse<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        LogFetchUsersStarted(logger, request.Page, request.PageSize, request.Active);

        var (users, totalCount) = await userRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Active,
            cancellationToken
        );

        var userResponses = users.ToResponseList().ToList();

        var pagedResponse = new PagedResponse<UserResponse>(
            userResponses,
            request.Page,
            request.PageSize,
            totalCount
        );

        LogFetchUsersSuccess(logger, users.Count, totalCount);

        return Result.Success(pagedResponse);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Buscando página {Page} (Tamanho: {PageSize}) de usuários. Filtrando apenas ativos: {Active}.")]
    private static partial void LogFetchUsersStarted(ILogger logger, int page, int pageSize, bool active);

    [LoggerMessage(Level = LogLevel.Information, Message = "Sucesso ao recuperar {Count} usuários para a página atual de um total de {TotalCount} registros.")]
    private static partial void LogFetchUsersSuccess(ILogger logger, int count, int totalCount);
}
