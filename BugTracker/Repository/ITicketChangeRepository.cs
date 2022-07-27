using BugTracker.Models;

namespace BugTracker.Repository
{
    public interface ITicketChangeRepository : IGenericRepository<TicketChange>
    {
        public void CreateTicketChange(Ticket ticket, string userId);
        public IEnumerable<TicketChange?> GetForTicket(int ticketId);
    }
}
