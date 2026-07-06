using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Messaging;
using FcgUsers.IoC;
using Microsoft.OpenApi;

namespace FcgUsers.Api.Extensions;

public static class ConfigureServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.AuthnAuthzConfig();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Cole somente o token JWT. O prefixo 'Bearer' será adicionado automaticamente."
            });

            // Exatamente como no Projeto 1, passando os parâmetros no construtor
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });

        // Seus serviços específicos
        services.ConfigureAppDependencies(configuration);
        services.AddMassTransitRabbitMqPublisher(configuration);
        services.AddHealthChecks().AddDbContextCheck<FcgUsersDbContext>();
    }
}
