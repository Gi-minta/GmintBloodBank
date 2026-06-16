using Domain.Entities.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Licensing;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.ToTable("FeatureFlags");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Key).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(250);
        builder.Property(e => e.Scope).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Key).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
