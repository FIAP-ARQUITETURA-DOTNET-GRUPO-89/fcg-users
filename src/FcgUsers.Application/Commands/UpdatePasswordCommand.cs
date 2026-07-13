using System.Text.Json.Serialization;
using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands;

public record UpdatePasswordCommand(
    Guid Id,
    string Password,
    Guid RequestUserId,
    string RequestUserEmail
) : IRequest<Result<UpdatePasswordResponse>>, IValidatableRequest
{
    [JsonIgnore]
    public Password NewPassword
    {
        get
        {
            FcgUsers.Domain.ValueObjects.Password.ValidarTextoPuro(Password);
            return FcgUsers.Domain.ValueObjects.Password.FromHash(Password);
        }
    }
}
