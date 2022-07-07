using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string RefLink { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
