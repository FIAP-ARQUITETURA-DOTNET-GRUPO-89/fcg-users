using FcgUsers.Application.Responses;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using MediatR;
using OperationResult;

namespace FcgUsers.Application.Queries.Users;

public record GetUsersByNameQuery(
    string Name,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResponse<UserResponse>>>, IValidatableRequest;
