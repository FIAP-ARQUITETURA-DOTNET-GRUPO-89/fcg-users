using FcgUsers.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FcgUsers.UnitTests.TestHelpers.Factories;

public static class InMemoryDbContextFactory
{
    public static FcgUsersDbContext CreateContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FcgUsersDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new FcgUsersDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}
