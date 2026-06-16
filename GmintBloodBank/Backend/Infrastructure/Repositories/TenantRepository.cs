using Domain.Entities.Tenancy;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TenantRepository : GenericRepository<Tenant>
{
    public TenantRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.Code == code && !t.IsDeleted, cancellationToken);
    }
}
