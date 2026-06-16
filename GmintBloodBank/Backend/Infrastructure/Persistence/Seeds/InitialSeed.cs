using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Seeds;

public static class InitialSeed
{
    public static async Task SeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();

        await CatalogSeed.SeedAsync(context);
        await SecuritySeed.SeedAsync(context);
        await TenancySeed.SeedAsync(context);
        await LicensingSeed.SeedAsync(context);
    }
}
