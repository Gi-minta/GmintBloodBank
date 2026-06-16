using Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Inventory;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasOne(e => e.BloodUnit).WithMany(bu => bu.Movements).HasForeignKey(e => e.BloodUnitId);
        builder.HasOne(e => e.MovementType).WithMany(mt => mt.Movements).HasForeignKey(e => e.MovementTypeId);
        builder.HasOne(e => e.PerformedBy).WithMany(s => s.Movements).HasForeignKey(e => e.PerformedById);
        builder.HasOne(e => e.FromLocation).WithMany().HasForeignKey(e => e.FromLocationId);
        builder.HasOne(e => e.ToLocation).WithMany().HasForeignKey(e => e.ToLocationId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
