using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands.Users;

public record UpdateUserRoleCommand(
    Guid Id,
    string RoleName
) : IRequest<Result<UpdateUserRoleResponse>>, IValidatableRequest;
