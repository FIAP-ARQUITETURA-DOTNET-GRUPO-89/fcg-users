using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserResponse>>, IValidatableRequest;
