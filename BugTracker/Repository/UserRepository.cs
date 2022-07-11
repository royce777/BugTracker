using BugTracker.Areas.Identity.Data;
using BugTracker.Data;

namespace BugTracker.Repository
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
