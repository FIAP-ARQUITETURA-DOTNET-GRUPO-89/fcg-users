using System.Text.Json.Serialization;
using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>, IValidatableRequest
{
    [JsonIgnore]
    public Email UserEmail => FcgUsers.Domain.ValueObjects.Email.Create(Email);
}
