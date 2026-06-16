using Domain.Entities.Donations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Donations;

public class DonationAppointmentConfiguration : IEntityTypeConfiguration<DonationAppointment>
{
    public void Configure(EntityTypeBuilder<DonationAppointment> builder)
    {
        builder.ToTable("DonationAppointments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Notes).HasColumnType("text");
        builder.HasOne(e => e.Donor).WithMany(d => d.Appointments).HasForeignKey(e => e.DonorId);
        builder.HasOne(e => e.BloodBank).WithMany(b => b.Appointments).HasForeignKey(e => e.BloodBankId);
        builder.HasOne(e => e.Status).WithMany(s => s.Appointments).HasForeignKey(e => e.StatusId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
