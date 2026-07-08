using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Infrastructure.Database;

namespace FcgUsers.IntegrationTests.TestHelpers;

/// <summary>
/// Responsável por popular o banco de dados com dados iniciais necessários para os testes de integração.
/// </summary>
public static class TestDataSeeder
{
    public static async Task SeedAsync(FcgUsersDbContext context)
    {
        // Se já houver usuários, não precisa popular novamente
        if (context.Users.Any())
        {
            return;
        }

        var user = new User(
        "Usuário Teste Integrado",
        new DateOnly(1990, 1, 1),
        Email.Create("teste@integracao.com"),
        Password.FromHash("hashedpassword123"),
        UserRole.User
        );

        context.Users.Add(user);

        await context.SaveChangesAsync();
    }
}
