using System.Net;
using System.Net.Http.Json;
using FcgUsers.IntegrationTests.Fixtures;
using FcgUsers.IntegrationTests.TestHelpers;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using Shouldly;
using Microsoft.EntityFrameworkCore;

namespace FcgUsers.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class CreateUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarUsuario_Entao_DeveRetornarCreatedEPersistirNoBanco()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var command = new
        {
            Name = "Test User",
            BirthDate = "1990-05-15",
            Email = "testuser@example.com",
            Password = "Password123!",
            Role = "Customer"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _fixture.ExecuteDbContextAsync<bool>(async db =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email.Address == "testuser@example.com");
            user.ShouldNotBeNull();
            user.Name.ShouldBe("Test User");
            user.Role.ToString().ShouldBe("Customer");
            return true;
        });
    }

    [Fact]
    public async Task Dado_EmailJaExistente_Quando_CriarUsuario_Entao_DeveRetornarBadRequest()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var targetEmail = "existinguser@example.com";

        await _fixture.ExecuteDbContextAsync<bool>(async db =>
        {
            var existingUser = new User(
                name: "Existing User",
                birthDate: new DateOnly(1990, 1, 1),
                email: Email.Create(targetEmail),
                password: Password.FromHash("HashedPassword123!"),
                userRole: UserRole.Customer
            );

            db.Users.Add(existingUser);
            await db.SaveChangesAsync();
            return true;
        });

        var command = new
        {
            Name = "New User",
            BirthDate = "1995-01-01",
            Email = targetEmail,
            Password = "Password123!",
            Role = "User"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
