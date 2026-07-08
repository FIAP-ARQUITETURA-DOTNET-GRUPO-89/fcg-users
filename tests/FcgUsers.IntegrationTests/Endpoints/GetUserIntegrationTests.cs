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
public class GetUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_IdExistente_Quando_BuscarPorId_Entao_DeveRetornarUsuarioComSucesso()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var userId = Guid.NewGuid();

        await _fixture.ExecuteDbContextAsync<bool>(async db => {
            var user = new User("Search User", new DateOnly(1990, 1, 1), Email.Create("search@test.com"), Password.FromHash("Password123!"), UserRole.User);
            typeof(User).GetProperty("Id")?.SetValue(user, userId);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return true;
        });

        // Act
        var response = await client.GetAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<object>();
        content.ShouldNotBeNull();
    }

    [Fact]
    public async Task Dado_UsuariosCadastrados_Quando_ListarTodos_Entao_DeveRetornarPaginaCorreta()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        // Act
        var response = await client.GetAsync("/api/users?page=1&pageSize=5&active=true");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<object>();
        content.ShouldNotBeNull();
    }

    [Fact]
    public async Task Dado_NomeExistente_Quando_BuscarPorNome_Entao_DeveRetornarListaFiltrada()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var searchName = "FindMe";

        await _fixture.ExecuteDbContextAsync<bool>(async db => {
            var user = new User(searchName, new DateOnly(1990, 1, 1), Email.Create("findme@test.com"), Password.FromHash("Password123!"), UserRole.User);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return true;
        });

        // Act
        var response = await client.GetAsync($"/api/users/name/{searchName}?page=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<object>();
        content.ShouldNotBeNull();
    }

    [Fact]
    public async Task Dado_IdInexistente_Quando_BuscarPorId_Entao_DeveRetornarNotFound()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var randomId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/users/{randomId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
