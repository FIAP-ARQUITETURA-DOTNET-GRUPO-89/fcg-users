using FluentValidation;
using MediatR;
using FcgUsers.Application;
using FcgUsers.Domain;
using FcgUsers.Domain.Repositories;
using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Messaging;
using FcgUsers.Infrastructure.Repositories;
using FcgUsers.Infrastructure.Services;
using FcgUsers.Application.Interfaces;
using FcgUsers.SharedKernel.Behaviors;
using FcgUsers.SharedKernel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FcgUsers.IoC;

public static class AppServiceCollectionExtensions
{
    public static void ConfigureAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>().Bind(configuration.GetSection("JwtSettings"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(IDomainEntryPoint).Assembly,
                typeof(IApplicationAssembly).Assembly,
                typeof(ValidationBehavior<,>).Assembly)
        );

        services.AddValidatorsFromAssemblyContaining<IApplicationAssembly>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Banco
        services.AddDbContext<FcgUsersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"),
                npgsql => npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null)));

        // MassTransit
        services.AddMassTransitRabbitMqPublisher(configuration);

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ISenhaHasherService, SenhaHasherService>();
    }
}
