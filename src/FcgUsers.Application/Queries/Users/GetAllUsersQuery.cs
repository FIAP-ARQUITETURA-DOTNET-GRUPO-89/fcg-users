using MediatR;
using FcgUsers.Application.Responses;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Queries.Users;

public record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 10,
    bool Active = true
) : IRequest<Result<PagedResponse<UserResponse>>>, IValidatableRequest;
