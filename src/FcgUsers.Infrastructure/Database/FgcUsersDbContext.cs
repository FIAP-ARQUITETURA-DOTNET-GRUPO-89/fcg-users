using FcgUsers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FcgUsers.Infrastructure.Database;

public class FcgUsersDbContext(DbContextOptions<FcgUsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgUsersDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        => configurationBuilder
            .Properties<string>()
            .AreUnicode(false)
            .HaveMaxLength(255);
}
