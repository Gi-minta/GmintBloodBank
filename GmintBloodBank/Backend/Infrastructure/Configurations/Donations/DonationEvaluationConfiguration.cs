using Domain.Entities.Donations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donations;

public class DonationEvaluationConfiguration : IEntityTypeConfiguration<DonationEvaluation>
{
    public void Configure(EntityTypeBuilder<DonationEvaluation> builder)
    {
        builder.ToTable("DonationEvaluations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.BloodPressure).HasMaxLength(20);
        builder.Property(e => e.Hemoglobin).HasColumnType("decimal(4,1)");
        builder.Property(e => e.WeightKg).HasColumnType("decimal(5,2)");
        builder.Property(e => e.Temperature).HasColumnType("decimal(4,1)");
        builder.Property(e => e.RejectionReason).HasColumnType("text");
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasOne(e => e.Donor).WithMany(d => d.Evaluations).HasForeignKey(e => e.DonorId);
        builder.HasOne(e => e.Doctor).WithMany(s => s.Evaluations).HasForeignKey(e => e.DoctorId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
