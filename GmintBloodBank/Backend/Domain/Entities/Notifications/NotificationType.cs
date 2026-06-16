using Domain.Entities.Common;

namespace Domain.Entities.Notifications;

public class NotificationType : AuditableEntity
{
    public string Code { get; set; }
    public string Description { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
