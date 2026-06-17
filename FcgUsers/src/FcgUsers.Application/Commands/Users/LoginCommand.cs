using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands.Users;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>, IValidatableRequest;
