using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands;

public record UpdateUserCommand(
    Guid Id,
    string Name,
    DateOnly BirthDate
) : IRequest<Result<UserResponse>>, IValidatableRequest;
