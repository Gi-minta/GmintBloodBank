using Domain.Entities.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Licensing;

public class LicensePlanConfiguration : IEntityTypeConfiguration<LicensePlan>
{
    public void Configure(EntityTypeBuilder<LicensePlan> builder)
    {
        builder.ToTable("LicensePlans");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(250);
        builder.Property(e => e.MonthlyPrice).HasColumnType("decimal(18,2)");
        builder.HasIndex(e => e.Name).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
