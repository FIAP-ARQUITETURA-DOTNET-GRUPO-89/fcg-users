using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Infrastructure.Repositories;
using FcgUsers.UnitTests.TestHelpers.Factories;
using Shouldly;

namespace FcgUsers.UnitTests.Infra.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task Dado_UsuarioValido_Quando_Adicionar_Entao_DevePersistir()
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var context = InMemoryDbContextFactory.CreateContext(connection);
        var repository = new UserRepository(context);
        var user = CreateUser("Iago Pachiani", "iago@teste.com");

        repository.Add(user);
        await repository.SaveChangesAsync();

        using var assertContext = InMemoryDbContextFactory.CreateContext(connection);
        var result = await assertContext.Users.FindAsync(user.Id);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Iago Pachiani");
    }

    [Fact]
    public async Task Dado_MultiplosUsuarios_Quando_BuscarPaginado_Entao_DeveRetornarPaginaCorreta()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();

        context.Users.AddRange(
            CreateUser("Usuario 1", "u1@teste.com"),
            CreateUser("Usuario 2", "u2@teste.com"),
            CreateUser("Usuario 3", "u3@teste.com"),
            CreateUser("Usuario 4", "u4@teste.com")
        );
        await context.SaveChangesAsync();
        var repository = new UserRepository(context);

        // Act
        var (items, total) = await repository.GetPagedAsync(page: 2, pageSize: 2, active: true);

        // Assert
        items.Count.ShouldBe(2);
        items.First().Name.ShouldBe("Usuario 3");
        items.Last().Name.ShouldBe("Usuario 4");
    }

    private static User CreateUser(string name, string email)
        => new(
            name,
            new DateOnly(1990, 1, 1),
            Email.Create(email),
            Password.FromHash("hashedpassword123"),
            UserRole.User
        );
}
