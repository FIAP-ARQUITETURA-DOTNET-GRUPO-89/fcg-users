using FcgUsers.Application;
using FcgUsers.Application.Interfaces;
using FcgUsers.Domain.Repositories;
using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Repositories;
using FcgUsers.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FcgUsers.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
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
