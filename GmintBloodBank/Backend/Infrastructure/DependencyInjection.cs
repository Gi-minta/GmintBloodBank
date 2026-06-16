using Application.Interfaces.Licensing;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Application.Interfaces.Tenancy;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<TenantSaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<TenantSaveChangesInterceptor>());
        });

        services.AddHttpContextAccessor();

        // Multi-tenant
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantProvider, TenantProvider>();

        // Licensing / Feature Flags
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<IFeatureFlagService, FeatureFlagService>();

        // Servicios generales
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IQrCodeService, QrCodeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        // Repositorios
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<DonorRepository>();
        services.AddScoped<DonationRepository>();
        services.AddScoped<BloodUnitRepository>();
        services.AddScoped<InventoryRepository>();
        services.AddScoped<TenantRepository>();
        services.AddScoped<LicenseRepository>();
        services.AddScoped<FeatureFlagRepository>();

        return services;
    }
}
