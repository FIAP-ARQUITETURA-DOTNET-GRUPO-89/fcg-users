using FcgUsers.Api.Endpoints;
using FcgUsers.Api.Extensions;
using FcgUsers.Api.Middlewares;
using FcgUsers.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});

builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddMassTransitRabbitMqPublisher(builder.Configuration);

var app = builder.Build();

// Middleware no topo
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUsersEndpoints();

app.Run();
