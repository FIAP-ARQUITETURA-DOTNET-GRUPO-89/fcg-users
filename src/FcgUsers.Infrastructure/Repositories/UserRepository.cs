using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Repositories;
using FcgUsers.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FcgUsers.Infrastructure.Repositories;

public class UserRepository(FcgUsersDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Set<User>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.Address.ToLower() == email.ToLower());

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _context.Set<User>().AnyAsync(u => u.Email.Address.ToLower() == email.ToLower());

    public async Task<(IReadOnlyCollection<User> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, bool active, CancellationToken ct = default)
    {
        var query = _context.Set<User>().Where(u => u.IsInactive != active);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task<(IReadOnlyCollection<User> Items, int TotalCount)> GetByNamePagedAsync(
        string name, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Set<User>().Where(u => u.Name.Contains(name));
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}
