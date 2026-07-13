using FcgUsers.Domain.Entities;

namespace FcgUsers.Application.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user);
}
