using FcgUsers.Application.Responses.Users;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Domain.Enums;
using Riok.Mapperly.Abstractions;
using FcgUsers.Application.Commands;

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

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email.Address,
            user.BirthDate,
            user.Role.ToString(),
            !user.IsInactive
        );
    }

    public static partial IEnumerable<UserResponse> ToResponseList(this IEnumerable<User> users);

    public static DeleteUserResponse ToDeleteResponse(this User user, string message)
    {
        return new DeleteUserResponse(
            user.Id,
            user.Name,
            !user.IsInactive,
            message
        );
    }

    public static UpdateUserRoleResponse ToUpdateRoleResponse(this User user, string message)
    {
        return new UpdateUserRoleResponse(
            user.Id,
            user.Role.ToString(),
            message
        );
    }
}
