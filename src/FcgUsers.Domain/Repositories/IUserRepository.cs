using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Interfaces.Repositories;

namespace FcgUsers.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);

    Task<(IReadOnlyCollection<User> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, bool active, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<User> Items, int TotalCount)> GetByNamePagedAsync(
        string name, int page, int pageSize, CancellationToken cancellationToken = default);
}
