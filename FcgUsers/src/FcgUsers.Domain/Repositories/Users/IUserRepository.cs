using FcgUsers.Domain.Entities;

namespace FcgUsers.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    void Add(User user);
    Task SaveChangesAsync();
}
