//using FcgUsers.IoC;
//using FcgUsers.Infrastructure.Messaging;
//using FcgUsers.Worker.Consumers;

//namespace FcgUsers.Worker.Extensions;

//public static class ConfigureServicesExtensions
//{
//    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
//    {
//        services.ConfigureWorkerDependencies(configuration);

//        services.AddMassTransitRabbitMq(configuration, x =>
//        {
//            x.AddConsumer<OrderPlacedConsumer>();
//        });

//        return services;
//    }
//}

using FcgUsers.Infrastructure.Messaging;
using FcgUsers.Worker.Consumers;
using FcgUsers.IoC;

public static class ConfigureServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureWorkerDependencies(configuration);

        services.AddMassTransitRabbitMq(configuration, x =>
        {
            x.AddConsumer<UserCreatedConsumer>();
        });

        return services;
    }
}
