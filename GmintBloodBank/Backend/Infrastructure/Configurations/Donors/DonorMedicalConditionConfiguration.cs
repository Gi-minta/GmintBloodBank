using Domain.Entities.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donors;

public class DonorMedicalConditionConfiguration : IEntityTypeConfiguration<DonorMedicalCondition>
{
    public void Configure(EntityTypeBuilder<DonorMedicalCondition> builder)
    {
        builder.ToTable("DonorMedicalConditions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasIndex(e => new { e.DonorId, e.MedicalConditionId }).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.Donor).WithMany(d => d.MedicalConditions).HasForeignKey(e => e.DonorId);
        builder.HasOne(e => e.MedicalCondition).WithMany(mc => mc.DonorMedicalConditions).HasForeignKey(e => e.MedicalConditionId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
