
namespace BugTracker.Models
{
    public class ManageProjectUsersViewModel
    {
        public ManageProjectUsersViewModel()
        {
            Members = new List<UserRoles>();
            Others = new List<UserRoles>();
        }
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public List<UserRoles> Members { get; set; }
        public List<UserRoles> Others { get; set; }

    }
}
