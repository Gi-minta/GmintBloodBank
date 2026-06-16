using Domain.Entities.BloodUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.BloodUnits;

public class BloodUnitStatusConfiguration : IEntityTypeConfiguration<BloodUnitStatus>
{
    public void Configure(EntityTypeBuilder<BloodUnitStatus> builder)
    {
        builder.ToTable("BloodUnitStatuses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(150).IsRequired();
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
