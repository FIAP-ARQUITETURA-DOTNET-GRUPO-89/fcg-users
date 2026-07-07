using System.Text.Json.Serialization;
using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Enums;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands;

public record UpdateUserRoleCommand(
    Guid Id,
    string RoleName
) : IRequest<Result<UpdateUserRoleResponse>>, IValidatableRequest
{
    [JsonIgnore]
    public UserRole Role => Enum.Parse<UserRole>(RoleName, ignoreCase: true);
}
