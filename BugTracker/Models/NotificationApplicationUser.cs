using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models
{
    public class NotificationApplicationUser
    {
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Read { get; set; } = false;
    }
}
