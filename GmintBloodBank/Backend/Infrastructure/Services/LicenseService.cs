using Application.Interfaces.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Valida el estado de licencia de un tenant contra la tabla Licenses/Subscriptions.
/// </summary>
public sealed class LicenseService : ILicenseService
{
    private readonly ApplicationDbContext _context;

    public LicenseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsLicenseActiveAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // TODO: validar contra Subscriptions/License (estado, fecha de expiración).
        return await Task.FromResult(true);
    }

    public async Task<bool> HasReachedUsageLimitAsync(Guid tenantId, string resourceKey, CancellationToken cancellationToken = default)
    {
        // TODO: validar límites del plan (ej. número de usuarios, sedes, unidades por mes).
        return await Task.FromResult(false);
    }
}
