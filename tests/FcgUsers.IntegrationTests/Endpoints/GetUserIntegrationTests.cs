using System.Net;
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
        // Mude de CreateUserCustomerAsync para CreateAdminClientAsync
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var userId = await _fixture.ExecuteDbContextAsync<Guid>(async db => {
            var user = new User("Search User", new DateOnly(1990, 1, 1),
                Email.Create("search@test.com"), Password.FromHash("Password123!"), UserRole.User);

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user.Id;
        });

        var response = await client.GetAsync($"/api/users/{userId}");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_IdInexistente_Quando_BuscarPorId_Entao_DeveRetornarNotFound()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var response = await client.GetAsync($"/api/users/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
