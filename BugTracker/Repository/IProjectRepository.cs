using BugTracker.Areas.Identity.Data;
using BugTracker.Models;

namespace BugTracker.Repository
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        bool IsInStaff(Project project, ApplicationUser user);
        Task<Project?> GetDetails(int projectId);
        Task<Project?> GetWithUsers(int projectId);
        //Implement
        Task<bool> HasPermission(Project project, string action);
    }
}
