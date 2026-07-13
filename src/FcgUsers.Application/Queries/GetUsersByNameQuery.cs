using MediatR;
using FcgUsers.Application.Responses;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Queries;

public record GetUsersByNameQuery(
    string Name,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResponse<UserResponse>>>, IValidatableRequest;
