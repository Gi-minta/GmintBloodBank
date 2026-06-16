using Domain.Entities.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class LicenseRepository : GenericRepository<License>
{
    public LicenseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<License?> GetActiveByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(l => l.TenantId == tenantId && l.IsActive && !l.IsDeleted, cancellationToken);
    }
}
