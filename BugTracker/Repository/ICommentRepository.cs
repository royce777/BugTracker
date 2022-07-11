using BugTracker.Models;

namespace BugTracker.Repository
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        IEnumerable<Comment> GetForTicket(int ticketId);
    }
}
