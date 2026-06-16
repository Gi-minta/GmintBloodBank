using Domain.Entities.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Licensing;

public class TenantFeatureConfiguration : IEntityTypeConfiguration<TenantFeature>
{
    public void Configure(EntityTypeBuilder<TenantFeature> builder)
    {
        builder.ToTable("TenantFeatures");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TenantId).IsRequired();
        builder.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId);
        builder.HasOne(e => e.FeatureFlag).WithMany(ff => ff.TenantFeatures).HasForeignKey(e => e.FeatureFlagId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
