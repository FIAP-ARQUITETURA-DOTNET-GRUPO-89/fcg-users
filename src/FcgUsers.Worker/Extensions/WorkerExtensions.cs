using FcgUsers.Worker.Consumers;
using MassTransit;

namespace FcgUsers.Worker.Extensions;

public static class WorkerExtensions
{
    public static IServiceCollection AddMassTransitWithConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Registra os consumidores localizados neste projeto
            x.AddConsumer<UserCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("rabbitmq")
                    ?? throw new InvalidOperationException("A ConnectionString 'rabbitmq' não foi encontrada.");

                cfg.Host(new Uri(connectionString));
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
