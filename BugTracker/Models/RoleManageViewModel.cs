using BugTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class RoleManageViewModel
    {
        public RoleManageViewModel()
        {
            Users = new List<ApplicationUser>();
            otherUsers = new List<ApplicationUser>();
        }

        public string Id { get; set; }
        [Required(ErrorMessage = "Role name is required !")]
        public string RoleName { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<ApplicationUser> otherUsers { get; set; }
    }
}
