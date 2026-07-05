using FcgUsers.Api.Extensions; // Onde está o seu ConfigureServices
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração de Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

// 2. Configuração centralizada de todos os serviços (incluindo nossa nova Infra)
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// 3. Configuração do Pipeline da aplicação
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
