namespace FcgUsers.Application.Responses.Users;

public record UserResponse(Guid Id, string Name, DateOnly BirthDate, string Email);
