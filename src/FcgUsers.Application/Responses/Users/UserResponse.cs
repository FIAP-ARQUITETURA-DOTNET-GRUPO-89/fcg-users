namespace FcgUsers.Application.Responses.Users;

public record UserResponse(Guid Id, string Name, string Email, DateOnly BirthDate, string Role, bool IsActive);
