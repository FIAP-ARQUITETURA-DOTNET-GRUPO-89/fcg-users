using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Infrastructure.Database;

namespace FcgUsers.IntegrationTests.TestHelpers;

public static class TestDataSeeder
{
    public static readonly User User = new(
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        "Usuário Teste Integrado",
        new DateOnly(1990, 1, 1),
        Email.Create("user@fcgusers.com"),
        Password.FromHash("hashedpassword123"),
        UserRole.Customer);

    public static readonly User Admin = new(
        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        "Administrador Teste",
        new DateOnly(1990, 1, 1),
        Email.Create("admin@fcgusers.com"),
        Password.FromHash("hashedpassword123"),
        UserRole.Admin);

    public static async Task SeedAsync(FcgUsersDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        context.Users.AddRange(User, Admin);

        await context.SaveChangesAsync();
    }
}
