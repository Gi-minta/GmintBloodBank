using Domain.Entities.BloodUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.BloodUnits;

public class BloodComponentConfiguration : IEntityTypeConfiguration<BloodComponent>
{
    public void Configure(EntityTypeBuilder<BloodComponent> builder)
    {
        builder.ToTable("BloodComponents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
