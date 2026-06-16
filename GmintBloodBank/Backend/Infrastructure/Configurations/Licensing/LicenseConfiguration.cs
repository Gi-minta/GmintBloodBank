using Domain.Entities.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Licensing;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("Licenses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.LicenseKey).HasMaxLength(200).IsRequired();
        builder.HasIndex(e => e.LicenseKey).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.Tenant).WithMany(t => t.Licenses).HasForeignKey(e => e.TenantId);
        builder.HasOne(e => e.Plan).WithMany(lp => lp.Licenses).HasForeignKey(e => e.LicensePlanId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
