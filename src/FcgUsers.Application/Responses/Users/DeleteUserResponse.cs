namespace FcgUsers.Application.Responses.Users;

public record DeleteUserResponse(Guid Id, string Name, bool IsActive, string Message);
