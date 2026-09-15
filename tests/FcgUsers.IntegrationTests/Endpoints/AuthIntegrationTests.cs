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
public class AuthIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_RealizarLogin_Entao_DeveRetornarToken()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var email = "login@teste.com";
        var passwordRaw = "Password123!";

        await _fixture.ExecuteDbContextAsync<bool>(async db => {
            var user = new User(
                "Login User",
                new DateOnly(1990, 1, 1),
                Email.Create(email),
                Password.FromHash(BCrypt.Net.BCrypt.HashPassword(passwordRaw)),
                UserRole.Customer
            );
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return true;
        });

        var command = new { Email = email, Password = passwordRaw };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<object>(cancellationToken: TestContext.Current.CancellationToken);
        content.ShouldNotBeNull();
    }

    [Fact]
    public async Task Dado_CredenciaisInvalidas_Quando_RealizarLogin_Entao_DeveRetornarUnauthorized()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var command = new { Email = "naoexiste@teste.com", Password = "WrongPassword!" };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
