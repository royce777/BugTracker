using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext db) : base(db)
        {
        }
        public override IEnumerable<Ticket> GetAll()
        {
            return _db.Tickets.Include(ticket => ticket.Project).ToList();
        }
        public IEnumerable<Ticket> GetForUser(ApplicationUser user)
        {
            return _db.Tickets.Include(ticket => ticket.Project).Where(ticket => ticket.Project.AssignedUsers.Contains(user));

        }

        public Task<Ticket?> GetWithProject(int ticketId)
        {
            return _db.Tickets.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public Task<Ticket?> GetWithUsers(int ticketId)
        {
            return _db.Tickets.Include(t => t.Project.AssignedUsers).FirstOrDefaultAsync(t => t.Id == ticketId);
        }
        public Task<Ticket?> GetDetails(int ticketId)
        {
            return _db.Tickets.Include(t=> t.Project.AssignedUsers).Include(t => t.Submitter).Include(t=> t.Developer).FirstOrDefaultAsync(t => t.Id == ticketId);

        }
    }
}
