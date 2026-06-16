using Domain.Entities.BloodUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.BloodUnits;

public class BloodUnitConfiguration : IEntityTypeConfiguration<BloodUnit>
{
    public void Configure(EntityTypeBuilder<BloodUnit> builder)
    {
        builder.ToTable("BloodUnits");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UnitCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.QrCode).HasMaxLength(200);
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasIndex(e => e.UnitCode).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.Donation).WithMany(d => d.BloodUnits).HasForeignKey(e => e.DonationId);
        builder.HasOne(e => e.BloodType).WithMany(bt => bt.BloodUnits).HasForeignKey(e => e.BloodTypeId);
        builder.HasOne(e => e.Component).WithMany(bc => bc.BloodUnits).HasForeignKey(e => e.ComponentId);
        builder.HasOne(e => e.Status).WithMany(bus => bus.BloodUnits).HasForeignKey(e => e.StatusId);
        builder.HasOne(e => e.StorageLocation).WithMany(sl => sl.BloodUnits).HasForeignKey(e => e.StorageLocationId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
