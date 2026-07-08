//using FcgUsers.Worker.Consumers;
//using FcgUsers.Worker.Extensions;

//using MassTransit;
//using Serilog;

//var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddSerilog((services, configuration) =>
//{
//    configuration
//        .ReadFrom.Configuration(builder.Configuration)
//        .ReadFrom.Services(services)
//        .Enrich.FromLogContext();
//});

//builder.AddServiceDefaults();

//builder.Services.ConfigureServices(builder.Configuration);

//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<UserCreatedConsumer>();

//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host(builder.Configuration["RabbitMq:Host"], "/");

//        cfg.ReceiveEndpoint("user-created-queue", e =>
//        {
//            e.ConfigureConsumer<UserCreatedConsumer>(context);
//        });
//    });
//});

//var host = builder.Build();
//host.Run();


using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.AddServiceDefaults();

builder.Services.ConfigureServices(builder.Configuration);

var host = builder.Build();

host.Run();
