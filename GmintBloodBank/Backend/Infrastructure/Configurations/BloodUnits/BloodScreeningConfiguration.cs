using Domain.Entities.BloodUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.BloodUnits;

public class BloodScreeningConfiguration : IEntityTypeConfiguration<BloodScreening>
{
    public void Configure(EntityTypeBuilder<BloodScreening> builder)
    {
        builder.ToTable("BloodScreenings");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.HivResult).HasMaxLength(20).IsRequired();
        builder.Property(e => e.HbsAgResult).HasMaxLength(20).IsRequired();
        builder.Property(e => e.HcvResult).HasMaxLength(20).IsRequired();
        builder.Property(e => e.VdrlResult).HasMaxLength(20).IsRequired();
        builder.Property(e => e.ChagasResult).HasMaxLength(20).IsRequired();
        builder.Property(e => e.RejectionReason).HasColumnType("text");
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasOne(e => e.BloodUnit).WithMany(bu => bu.Screenings).HasForeignKey(e => e.BloodUnitId);
        builder.HasOne(e => e.Technician).WithMany(s => s.Screenings).HasForeignKey(e => e.TechnicianId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
