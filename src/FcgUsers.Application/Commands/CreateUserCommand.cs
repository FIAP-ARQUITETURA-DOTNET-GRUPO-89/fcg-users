using System.Text.Json.Serialization;
using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands;

public record CreateUserCommand(
    string Name,
    DateOnly BirthDate,
    string Email,
    string Password,
    string Role
) : IRequest<Result<CreateUserResponse>>, IValidatableRequest
{
    [JsonIgnore]
    public Email UserEmail => FcgUsers.Domain.ValueObjects.Email.Create(Email);

    [JsonIgnore]
    public Password UserPassword
    {
        get
        {
            FcgUsers.Domain.ValueObjects.Password.ValidarTextoPuro(Password);

            return FcgUsers.Domain.ValueObjects.Password.FromHash(Password);
        }
    }
}
