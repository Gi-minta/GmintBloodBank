using Domain.Entities.BloodUnits;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class BloodUnitRepository : GenericRepository<BloodUnit>
{
    public BloodUnitRepository(ApplicationDbContext context) : base(context) { }

    public async Task<BloodUnit?> GetByUnitCodeAsync(string unitCode, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(b => b.UnitCode == unitCode && !b.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<BloodUnit>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(b => b.IsReleased && !b.IsDeleted && b.ExpirationDate > DateTime.UtcNow).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BloodUnit>> GetExpiringAsync(int days, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddDays(days);
        return await DbSet.Where(b => b.ExpirationDate <= threshold && !b.IsDeleted).ToListAsync(cancellationToken);
    }
}
