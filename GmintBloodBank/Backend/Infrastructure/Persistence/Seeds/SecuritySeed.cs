using Domain.Entities.Security;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeds;

public static class SecuritySeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRoles(context);
        await SeedUsers(context);
    }

    private static async Task SeedRoles(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new() { Name = "Administrator", Description = "Acceso completo al sistema" },
            new() { Name = "Doctor", Description = "Validación médica y aprobaciones" },
            new() { Name = "Nurse", Description = "Atención de donantes" },
            new() { Name = "Technician", Description = "Laboratorio y análisis" },
            new() { Name = "Receptionist", Description = "Citas y atención al público" },
        };

        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Administrator");

        var users = new List<User>
        {
            new()
            {
                RoleId = adminRole.Id,
                Username = "admin",
                Email = "admin@gmintbloodbank.com",
                PasswordHash = PasswordHasher.Hash("Admin123!"),
                IsActive = true,
            },
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
