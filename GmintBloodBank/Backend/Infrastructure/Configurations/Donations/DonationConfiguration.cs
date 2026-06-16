using Domain.Entities.Donations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donations;

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("Donations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.DonationCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CollectionBagCode).HasMaxLength(100);
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasIndex(e => e.DonationCode).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.Donor).WithMany(d => d.Donations).HasForeignKey(e => e.DonorId);
        builder.HasOne(e => e.BloodBank).WithMany(b => b.Donations).HasForeignKey(e => e.BloodBankId);
        builder.HasOne(e => e.Evaluation).WithMany().HasForeignKey(e => e.EvaluationId);
        builder.HasOne(e => e.Status).WithMany(s => s.Donations).HasForeignKey(e => e.StatusId);
        builder.HasOne(e => e.PerformedByStaff).WithMany(s => s.PerformedDonations).HasForeignKey(e => e.PerformedByStaffId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
