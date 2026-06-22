using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var isTesting = builder.Environment.IsEnvironment("Testing");

var postgres = isTesting
    ? builder.AddPostgres("Postgres")
        .WithLifetime(ContainerLifetime.Session)
        .AddDatabase("Default", "fcgusers-db")
    : builder.AddPostgres("Postgres", port: 5432)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithPgAdmin(c => c.WithLifetime(ContainerLifetime.Persistent))
        .AddDatabase("Default", "fcgusers-db");

var rabbitmq = isTesting
    ? builder.AddRabbitMQ("rabbitmq")
        .WithManagementPlugin()
        .WithLifetime(ContainerLifetime.Session)
    : builder.AddRabbitMQ("rabbitmq")
        .WithManagementPlugin()
        .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.FcgUsers_Api>("fcgusers-api")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
        .WithReference(postgres)
        .WithReference(rabbitmq)
        .WaitFor(postgres)
        .WaitFor(rabbitmq);

builder.AddProject<Projects.FcgUsers_Worker>("fcgusers-worker")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
        .WithReference(postgres)
        .WithReference(rabbitmq)
        .WaitFor(postgres)
        .WaitFor(rabbitmq);

builder.Build().Run();
