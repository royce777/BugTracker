using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models
{
    public class UserRoles
    {
        public ApplicationUser User;
        public List<string> Roles;
    }
}
