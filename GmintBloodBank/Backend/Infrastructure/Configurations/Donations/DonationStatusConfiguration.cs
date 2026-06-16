using Domain.Entities.Donations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donations;

public class DonationStatusConfiguration : IEntityTypeConfiguration<DonationStatus>
{
    public void Configure(EntityTypeBuilder<DonationStatus> builder)
    {
        builder.ToTable("DonationStatuses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(150).IsRequired();
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
