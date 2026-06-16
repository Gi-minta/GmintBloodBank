using Domain.Entities.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class FeatureFlagRepository : GenericRepository<FeatureFlag>
{
    public FeatureFlagRepository(ApplicationDbContext context) : base(context) { }

    public async Task<FeatureFlag?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(f => f.Key == key && !f.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<FeatureFlag>> GetGlobalFlagsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(f => f.Scope == "GLOBAL" && !f.IsDeleted).ToListAsync(cancellationToken);
    }
}
