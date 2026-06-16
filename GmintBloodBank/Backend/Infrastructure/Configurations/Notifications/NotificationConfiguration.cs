using Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Subject).HasMaxLength(250).IsRequired();
        builder.Property(e => e.Body).HasColumnType("text").IsRequired();
        builder.HasOne(e => e.Type).WithMany(nt => nt.Notifications).HasForeignKey(e => e.TypeId);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
