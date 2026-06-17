using MediatR;
using FcgUsers.Application.Queries.Users;
using FcgUsers.Application.Mappers.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Repositories;
using FcgUsers.SharedKernel.Exceptions;
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
            return new NotFoundException($"User with ID {request.Id} was not found.");
        }

        LogFetchUserByIdSuccess(logger, user.Id);

        var response = user.ToResponse();
        return Result.Success(response);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Fetching user details for ID '{userId}'.")]
    private static partial void LogFetchUserByIdStarted(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Fetch failed. User with ID '{userId}' was not found.")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Successfully retrieved details for user ID '{userId}'.")]
    private static partial void LogFetchUserByIdSuccess(ILogger logger, Guid userId);
}
