using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FcgUsers.Application.Interfaces;
using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Services;
using FcgUsers.IntegrationTests.TestHelpers;
using FcgUsers.SharedKernel.Settings;
using FcgUsers.Worker.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace FcgUsers.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;
    private TestDatabaseManager _dbManager = default!;
    private string _connectionString = string.Empty;

    public async ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FcgUsers_AppHost>();

        // Força a leitura do appsettings.Testing.json na pasta do projeto de testes
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Testing.json");
        builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<UserCreatedConsumer>();
            x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
        });

        App = await builder.BuildAsync();
        await App.StartAsync();

        _connectionString = await App.GetConnectionStringAsync("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' não encontrada.");

        _dbManager = new TestDatabaseManager(_connectionString);
        await _dbManager.InitializeAsync();
        await _dbManager.ResetAsync();
    }

    public async Task ResetDatabaseAsync() => await _dbManager.ResetAsync();

    public async Task<T> ExecuteDbContextAsync<T>(Func<FcgUsersDbContext, Task<T>> action)
    {
        var options = new DbContextOptionsBuilder<FcgUsersDbContext>().UseNpgsql(_connectionString).Options;
        await using var context = new FcgUsersDbContext(options);
        return await action(context);
    }

    public HttpClient CreateClient() => App.CreateHttpClient("fcgusers-api", endpointName: "https");

    public async ValueTask DisposeAsync()
    {
        if (App is not null)
        {
            await App.StopAsync();
            await App.DisposeAsync();
        }
    }
}
