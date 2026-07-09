using System.Net;
using FcgUsers.IntegrationTests.Fixtures;
using FcgUsers.IntegrationTests.TestHelpers;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Domain.Enums;
using Shouldly;

namespace FcgUsers.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class DeleteUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_DeletarUsuario_Entao_DeveDesativarNoBanco()
    {
        // Arrange
        var client = TestAuthHelper.CreateAdminClientAsync(_fixture).Result;
        var userId = Guid.NewGuid();

        await _fixture.ExecuteDbContextAsync<bool>(async db =>
        {
            var user = new User(
                "Usuário para Deletar",
                new DateOnly(1990, 1, 1),
                Email.Create("deletar@teste.com"),
                Password.FromHash("Password123!"),
                UserRole.User
            );

            typeof(User).GetProperty("Id")?.SetValue(user, userId);

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return true;
        });

        // Act
        var response = await client.DeleteAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _fixture.ExecuteDbContextAsync<bool>(async db =>
        {
            var user = await db.Users.FindAsync(userId);
            user.ShouldNotBeNull();
            user.IsInactive.ShouldBe(true);
            return true;
        });
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_DeletarUsuario_Entao_DeveRetornarNotFound()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/users/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound); // Alterado de BadRequest para NotFound
    }
}
