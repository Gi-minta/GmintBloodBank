using Domain.Entities.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Tenancy;

public class BloodBankConfiguration : IEntityTypeConfiguration<BloodBank>
{
    public void Configure(EntityTypeBuilder<BloodBank> builder)
    {
        builder.ToTable("BloodBanks");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PhoneNumber).HasMaxLength(30);
        builder.Property(e => e.Email).HasMaxLength(150);
        builder.HasIndex(e => e.Code).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.Tenant).WithMany(t => t.BloodBanks).HasForeignKey(e => e.TenantId);
        builder.HasOne(e => e.Address).WithMany(a => a.BloodBanks).HasForeignKey(e => e.AddressId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
