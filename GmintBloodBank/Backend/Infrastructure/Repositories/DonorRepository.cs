using Application.Interfaces.Persistence;
using Domain.Entities.Donors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DonorRepository : GenericRepository<Donor>
{
    public DonorRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Donor?> GetByDonorCodeAsync(string donorCode, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.DonorCode == donorCode && !d.IsDeleted, cancellationToken);
    }

    public async Task<Donor?> GetByIdentificationAsync(string identification, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.Identification == identification && !d.IsDeleted, cancellationToken);
    }
}
