using Domain.Entities.Inventory;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class InventoryRepository : GenericRepository<InventoryMovement>
{
    public InventoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InventoryMovement>> GetByBloodUnitIdAsync(Guid bloodUnitId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(m => m.BloodUnitId == bloodUnitId && !m.IsDeleted).OrderByDescending(m => m.MovementDate).ToListAsync(cancellationToken);
    }
}
