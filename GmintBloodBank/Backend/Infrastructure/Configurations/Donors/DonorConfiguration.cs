using Domain.Entities.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donors;

public class DonorConfiguration : IEntityTypeConfiguration<Donor>
{
    public void Configure(EntityTypeBuilder<Donor> builder)
    {
        builder.ToTable("Donors");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.DonorCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Identification).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PhoneNumber).HasMaxLength(30);
        builder.Property(e => e.Email).HasMaxLength(150);
        builder.HasIndex(e => e.DonorCode).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasIndex(e => e.Identification).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.BloodType).WithMany(bt => bt.Donors).HasForeignKey(e => e.BloodTypeId);
        builder.HasOne(e => e.Gender).WithMany(g => g.Donors).HasForeignKey(e => e.GenderId);
        builder.HasOne(e => e.Address).WithMany(a => a.Donors).HasForeignKey(e => e.AddressId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
