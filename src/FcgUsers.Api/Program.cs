using FcgUsers.Api.Endpoints;
using FcgUsers.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});

// 2. Centralização de serviços (Inclui Auth, Swagger, MassTransit, Dependências)
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// 3. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

// 4. Mapeamento
app.MapAuthEndpoints();
app.MapUsersEndpoints();

app.Run();
