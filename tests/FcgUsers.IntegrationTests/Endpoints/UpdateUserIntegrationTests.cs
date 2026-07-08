using System.Net;
using System.Net.Http.Json;
using FcgUsers.IntegrationTests.Fixtures;
using FcgUsers.IntegrationTests.TestHelpers;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Domain.Enums;
using Shouldly;

namespace FcgUsers.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class UpdateUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarPerfil_Entao_DevePersistirNomeEBirthDate()
    {
        // Arrange
        var client = TestAuthHelper.CreateAdminClientAsync(_fixture).Result;
        var userId = Guid.NewGuid();

        await _fixture.ExecuteDbContextAsync<bool>(async db => {
            var user = new User("Nome Antigo", new DateOnly(1990, 1, 1), Email.Create("update@teste.com"), Password.FromHash("Password123!"), UserRole.User);
            typeof(User).GetProperty("Id")?.SetValue(user, userId);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return true;
        });

        var command = new { Id = userId, Name = "Nome Novo", BirthDate = "1995-05-05" };

        // Act
        var response = await client.PutAsJsonAsync($"/api/users/{userId}", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await _fixture.ExecuteDbContextAsync<bool>(async db => {
            var user = await db.Users.FindAsync(userId);
            user.ShouldNotBeNull();
            user.Name.ShouldBe("Nome Novo");
            return true;
        });
    }

    [Fact]
    public async Task Dado_Admin_Quando_TentarRebaixarParaUser_Entao_DeveRetornarBadRequest()
    {
        // Arrange
        var client = TestAuthHelper.CreateAdminClientAsync(_fixture).Result;
        var adminId = Guid.NewGuid();

        await _fixture.ExecuteDbContextAsync<bool>(async db => {
            var admin = new User("Admin", new DateOnly(1990, 1, 1), Email.Create("admin@teste.com"), Password.FromHash("Password123!"), UserRole.Admin);
            typeof(User).GetProperty("Id")?.SetValue(admin, adminId);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
            return true;
        });

        var command = new { Id = adminId, RoleName = "User" };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/users/{adminId}/role", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
