using Domain.Entities.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donors;

public class BloodTypeConfiguration : IEntityTypeConfiguration<BloodType>
{
    public void Configure(EntityTypeBuilder<BloodType> builder)
    {
        builder.ToTable("BloodTypes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).HasMaxLength(5).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
