using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Domain.Enums;
using Riok.Mapperly.Abstractions;

namespace FcgUsers.Application.Mappers.Users;

[Mapper]
public static partial class UserMapper
{
    public static User ToEntity(this CreateUserCommand command)
    {
        var email = Email.Create(command.Email);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var password = Password.FromHash(passwordHash);

        return new User(
            command.Name,
            command.BirthDate,
            email,
            password,
            Enum.Parse<UserRole>(command.Role)
        );
    }

    public static CreateUserResponse ToCreateUserResponse(this User user)
        => new(user.Id, user.Name, user.BirthDate, user.Email.Address);

    [MapProperty("Email.Address", nameof(UserResponse.Email))]
    public static partial UserResponse ToResponse(this User user);

    public static partial IEnumerable<UserResponse> ToResponseList(this IEnumerable<User> users);
}
