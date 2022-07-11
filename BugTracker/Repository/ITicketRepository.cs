using BugTracker.Areas.Identity.Data;
using BugTracker.Models;

namespace BugTracker.Repository
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        IEnumerable<Ticket> GetForUser(ApplicationUser user);
        Task<Ticket?> GetWithProject(int ticketId);
        Task<Ticket?> GetWithUsers(int ticketId);
        Task<Ticket?> GetDetails(int ticketId);
    }
}
