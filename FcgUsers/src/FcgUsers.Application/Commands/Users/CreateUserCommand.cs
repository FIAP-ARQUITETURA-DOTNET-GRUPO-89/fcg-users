using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands.Users;

public record CreateUserCommand(
    string Name,
    DateOnly BirthDate,
    string Email,
    string Password,
    string Role
) : IRequest<Result<CreateUserResponse>>, IValidatableRequest;
