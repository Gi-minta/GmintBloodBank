using Domain.Entities.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Tenancy;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EmployeeCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Identification).HasMaxLength(50);
        builder.Property(e => e.PhoneNumber).HasMaxLength(30);
        builder.HasIndex(e => e.EmployeeCode).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.BloodBank).WithMany(b => b.StaffMembers).HasForeignKey(e => e.BloodBankId);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Category).WithMany(c => c.StaffMembers).HasForeignKey(e => e.CategoryId);
        builder.HasOne(e => e.Address).WithMany(a => a.StaffMembers).HasForeignKey(e => e.AddressId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
