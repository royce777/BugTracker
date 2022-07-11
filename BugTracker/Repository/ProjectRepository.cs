using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext db) : base(db)
        {
        }

        public Task<Project?> GetDetails(int projectId)
        {
            return _db.Projects.Include(p => p.AssignedUsers).Include(p => p.Tickets).FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public Task<Project?> GetWithUsers(int projectId)
        {
            return _db.Projects.Include(p => p.AssignedUsers).FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public Task<bool> HasPermission(Project project, string action)
        {
            throw new NotImplementedException();
        }

        public bool IsInStaff(Project project, ApplicationUser user)
        {
            if(project.AssignedUsers!= null)
            {
                return project.AssignedUsers.Contains(user);
            }
            throw new NullReferenceException("IsInStaff Project.AssignedUsers is null !");
        }
    }
}
