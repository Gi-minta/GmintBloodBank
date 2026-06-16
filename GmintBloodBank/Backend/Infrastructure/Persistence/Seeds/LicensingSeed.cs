using Domain.Entities.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeds;

public static class LicensingSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.FeatureFlags.AnyAsync()) return;

        var plan = new LicensePlan
        {
            Name = "Básico",
            Description = "Plan básico para bancos de sangre pequeños",
            MonthlyPrice = 0,
            MaxTenants = 1,
            MaxUsers = 10,
        };

        context.LicensePlans.Add(plan);
        await context.SaveChangesAsync();

        var flags = new List<FeatureFlag>
        {
            new() { Key = "DONOR_MANAGEMENT", Description = "Gestión de donantes", IsEnabled = true, Scope = "GLOBAL" },
            new() { Key = "DONATION_SCHEDULING", Description = "Agenda de donaciones", IsEnabled = true, Scope = "GLOBAL" },
            new() { Key = "LAB_SCREENING", Description = "Screening de laboratorio", IsEnabled = true, Scope = "GLOBAL" },
            new() { Key = "INVENTORY_TRACKING", Description = "Trazabilidad de inventario", IsEnabled = true, Scope = "GLOBAL" },
            new() { Key = "REPORTS", Description = "Reportes y estadísticas", IsEnabled = true, Scope = "GLOBAL" },
            new() { Key = "QR_CODES", Description = "Generación de códigos QR", IsEnabled = false, Scope = "GLOBAL" },
        };

        context.FeatureFlags.AddRange(flags);
        await context.SaveChangesAsync();
    }
}
