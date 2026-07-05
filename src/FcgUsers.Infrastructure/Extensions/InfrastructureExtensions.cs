using FcgUsers.Application.Interfaces;
using FcgUsers.Domain.Repositories;
using FcgUsers.Infrastructure.Database; // Para o FcgUsersDbContext
using FcgUsers.Infrastructure.Repositories; // Ajuste para o namespace correto dos seus repositórios
using FcgUsers.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FcgUsers.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ISenhaHasherService, SenhaHasherService>();

        services.AddDbContext<FcgUsersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"),
                npgsql => npgsql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null)));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
