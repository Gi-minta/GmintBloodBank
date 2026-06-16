using Domain.Entities.Tenancy;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeds;

public static class TenancySeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Tenants.AnyAsync()) return;

        var address = new Address
        {
            Country = "México",
            City = "Ciudad de México",
            AddressLine1 = "Av. Principal 123",
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        var tenant = new Tenant
        {
            Code = "DEFAULT",
            Name = "Banco de Sangre Principal",
            IsActive = true,
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var bloodBank = new BloodBank
        {
            TenantId = tenant.Id,
            Code = "BB001",
            Name = "Banco de Sangre Central",
            AddressId = address.Id,
            IsActive = true,
        };

        context.BloodBanks.Add(bloodBank);
        await context.SaveChangesAsync();
    }
}
