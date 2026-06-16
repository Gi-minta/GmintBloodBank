using Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Inventory;

public class InventoryMovementTypeConfiguration : IEntityTypeConfiguration<InventoryMovementType>
{
    public void Configure(EntityTypeBuilder<InventoryMovementType> builder)
    {
        builder.ToTable("InventoryMovementTypes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(150).IsRequired();
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
