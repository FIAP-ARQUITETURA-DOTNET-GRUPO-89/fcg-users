namespace FcgUsers.Application.Responses.Users;

public record CreateUserResponse(
    Guid Id,
    string Name,
    DateOnly BirthDate,
    string Email
);
