using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Messaging;
using FcgUsers.IoC;
using Microsoft.OpenApi;

namespace FcgUsers.Api.Extensions;

public static class ConfigureServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AuthnAuthzConfig();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Entre com o token JWT (ex: Bearer {token})",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("bearer", securityScheme);

            // Em vez de usar Reference = new OpenApiReference que está falhando,
            // usamos um OpenApiSecurityRequirement que associa o esquema pelo ID.
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });

        services.ConfigureAppDependencies(configuration);
        services.AddMassTransitRabbitMqPublisher(configuration);
        services.AddHealthChecks().AddDbContextCheck<FcgUsersDbContext>();
    }
}
