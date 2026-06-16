using Domain.Entities.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Tenancy;

public class StaffCategoryConfiguration : IEntityTypeConfiguration<StaffCategory>
{
    public void Configure(EntityTypeBuilder<StaffCategory> builder)
    {
        builder.ToTable("StaffCategories");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(250);
        builder.HasIndex(e => e.Name).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
