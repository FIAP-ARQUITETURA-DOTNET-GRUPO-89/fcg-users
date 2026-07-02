using FcgUsers.Domain.Entities;

namespace FcgUsers.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    void Add(User user);
    Task<User?> GetByIdAsync(Guid id);
    void Update(User user);
    Task SaveChangesAsync();

    Task<(IReadOnlyCollection<User> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool active,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<User> Items, int TotalCount)> GetByNamePagedAsync(
        string name,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
