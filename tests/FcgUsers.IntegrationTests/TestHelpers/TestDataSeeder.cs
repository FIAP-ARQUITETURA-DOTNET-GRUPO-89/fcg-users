using FcgUsers.Domain.Entities;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Domain.Enums;
using FcgUsers.Infrastructure.Database;

namespace FcgUsers.IntegrationTests.TestHelpers;

public static class TestDataSeeder
{
    public static async Task SeedAsync(FcgUsersDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var user = new User(
            "Usuário Teste Integrado",
            new DateOnly(1990, 1, 1),
            Email.Create("user@fcgusers.com"),
            Password.FromHash("hashedpassword123"),
            UserRole.User
        );

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
