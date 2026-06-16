using Domain.Entities.Donations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DonationRepository : GenericRepository<Donation>
{
    public DonationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Donation?> GetByDonationCodeAsync(string donationCode, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.DonationCode == donationCode && !d.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Donation>> GetByDonorIdAsync(Guid donorId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(d => d.DonorId == donorId && !d.IsDeleted).ToListAsync(cancellationToken);
    }
}
