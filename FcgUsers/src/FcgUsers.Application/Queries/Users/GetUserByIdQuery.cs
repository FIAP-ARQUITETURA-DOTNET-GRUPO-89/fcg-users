using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using MediatR;
using OperationResult;

namespace FcgUsers.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserResponse>>, IValidatableRequest;
