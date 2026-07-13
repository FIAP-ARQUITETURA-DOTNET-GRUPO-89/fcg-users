using FcgUsers.Application.Interfaces;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FcgUsers.Infrastructure.Seed;

/// <summary>
/// Responsável por popular o banco de dados com dados iniciais utilizados exclusivamente em ambiente de desenvolvimento.
/// </summary>
public static class DevDatabaseSeeder
{
    public static async Task SeedAsync(FcgUsersDbContext context, ISenhaHasherService senhaHasher)
    {
        var emails = new[]
        {
            "maria.silva@email.com",
            "joao.silva@email.com"
        };

        var usuariosExistentes = await context.Users
            .Where(u => emails.Contains(u.Email.Address))
            .AnyAsync();

        if (usuariosExistentes)
        {
            return;
        }

        var usuarios = new List<User>
        {
            new(
                name: "Maria Silva",
                birthDate: new DateOnly(1990, 1, 1),
                email: Email.Create("maria.silva@email.com"),
                password: Password.FromHash(senhaHasher.Hash("Abc!1234")),
                userRole: UserRole.Admin
            ),
            new(
                name: "João Silva",
                birthDate: new DateOnly(1995, 1, 1),
                email: Email.Create("joao.silva@email.com"),
                password: Password.FromHash(senhaHasher.Hash("Abc!1234")),
                userRole: UserRole.User
            )
        };

        await context.Users.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();
    }
}

