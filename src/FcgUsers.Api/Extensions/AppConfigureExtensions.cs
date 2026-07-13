using FcgUsers.Api.Endpoints;
using FcgUsers.Api.Middlewares;
using FcgUsers.Application.Interfaces;
using FcgUsers.Infrastructure.Database;
using FcgUsers.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

namespace FcgUsers.Api.Extensions;

public static class AppConfigureExtensions
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();


        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            c.RoutePrefix = "";
        });

        var runMigration = app.Environment.IsDevelopment() ||
                   (app.Environment.IsProduction() && Environment.GetEnvironmentVariable("RUN_MIGRATION") == "true");

        if (runMigration)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<FcgUsersDbContext>();
            var senhaHasher = scope.ServiceProvider.GetRequiredService<ISenhaHasherService>();
            if (db.Database.IsRelational())
            {
                db.Database.Migrate();
                await DevDatabaseSeeder.SeedAsync(db, senhaHasher);
            }
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthEndpoints();
        app.MapUsersEndpoints();

        app.MapHealthChecks("/health");
        app.MapHealthChecks("/ready");
    }
}
