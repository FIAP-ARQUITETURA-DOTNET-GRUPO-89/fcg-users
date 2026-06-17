using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands.Users;

public record UpdatePasswordCommand(
    Guid Id,
    string Password,
    Guid RequestUserId,
    string RequestUserEmail
) : IRequest<Result<UpdatePasswordResponse>>, IValidatableRequest;
