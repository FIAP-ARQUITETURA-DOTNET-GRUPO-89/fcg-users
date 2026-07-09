using FcgUsers.Application.Interfaces;
using FcgUsers.Domain.Repositories;
using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Repositories;
using FcgUsers.Infrastructure.Services;
using MassTransit;
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

    public static IServiceCollection AddMassTransitRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        // Esta linha verifica se o IBus (interface principal do MassTransit) já foi registrado.
        // Se já foi, ele pula o registro, evitando o erro de "already called".
        if (services.Any(s => s.ServiceType == typeof(IBus)))
        {
            return services;
        }

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("rabbitmq")
                    ?? throw new InvalidOperationException("A ConnectionString 'rabbitmq' não foi encontrada.");

                cfg.Host(new Uri(connectionString));
            });
        });

        return services;
    }
}
