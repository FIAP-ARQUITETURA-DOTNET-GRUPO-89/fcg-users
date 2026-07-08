using System.Net.Http.Headers;
using FcgUsers.IntegrationTests.Fixtures;
using FcgUsers.Application.Interfaces; // Importante para achar o ITokenService
using Microsoft.Extensions.DependencyInjection;

namespace FcgUsers.IntegrationTests.TestHelpers;

/// <summary>
/// Responsável por facilitar a criação de HttpClients autenticados para testes de integração.
/// </summary>
public static class TestAuthHelper
{
    public const string UserEmail = "user@fcgusers.com";
    public const string AdminEmail = "admin@fcgusers.com";

    /// <summary>
    /// Cria um HttpClient autenticado como usuário Admin.
    /// </summary>
    public static async Task<HttpClient> CreateAdminClientAsync(IntegrationTestFixture fixture)
    {
        var client = fixture.CreateClient();
        var tokenService = fixture.App.Services.GetRequiredService<ITokenService>();

        var token = tokenService.GenerateJwtToken(AdminEmail, "Admin");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    /// <summary>
    /// Cria um HttpClient autenticado como usuário padrão (User).
    /// </summary>
    public static async Task<HttpClient> CreateUserCustomerAsync(IntegrationTestFixture fixture)
    {
        var client = fixture.CreateClient();
        var tokenService = fixture.App.Services.GetRequiredService<ITokenService>();

        var token = tokenService.GenerateJwtToken(UserEmail, "User");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    /// <summary>
    /// Cria um HttpClient sem autenticação.
    /// </summary>
    public static HttpClient CreateAnonymousClient(IntegrationTestFixture fixture)
        => fixture.CreateClient();
}
