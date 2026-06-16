using Application.Interfaces.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Resuelve Feature Flags combinando configuración global (FeatureFlags)
/// con overrides por tenant (TenantFeatures).
/// </summary>
public sealed class FeatureFlagService : IFeatureFlagService
{
    private readonly ApplicationDbContext _context;

    public FeatureFlagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsEnabledAsync(string featureKey, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        // TODO: 1) buscar override en TenantFeatures para tenantId
        //       2) si no existe, usar el valor por defecto de FeatureFlags
        return await Task.FromResult(false);
    }

    public async Task<IReadOnlyDictionary<string, bool>> GetAllFlagsAsync(Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        // TODO: combinar FeatureFlags globales + TenantFeatures del tenant.
        return await Task.FromResult<IReadOnlyDictionary<string, bool>>(new Dictionary<string, bool>());
    }
}
