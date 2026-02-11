using Microsoft.EntityFrameworkCore;
using Persistence.IdentityData.DbContext;

namespace LungDiseases.Extensions
{
    public static class WebApplicationRegistration
    {
        public static async Task<WebApplication> MigrateIdentityDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContextService = scope.ServiceProvider.GetRequiredService<LungIdentityDbContext>();
            var PendingMigration = await dbContextService.Database.GetPendingMigrationsAsync();
            if (PendingMigration.Any())
                await dbContextService.Database.MigrateAsync();
            return app;
        }

    }
}
