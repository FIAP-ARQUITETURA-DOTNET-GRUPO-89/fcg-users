using FcgUsers.Api.Endpoints;
using FcgUsers.Api.Extensions;
using FcgUsers.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});

// 2. Serviços (Atenção: verifique se ConfigureServices não chama o MassTransit internamente)
builder.Services.ConfigureServices(builder.Configuration);

// 3. Configuração de Mensageria (Apenas Publisher)
builder.Services.AddMassTransitRabbitMqPublisher(builder.Configuration);

var app = builder.Build();

// 4. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapeamento
app.MapAuthEndpoints();
app.MapUsersEndpoints();

app.Run();
