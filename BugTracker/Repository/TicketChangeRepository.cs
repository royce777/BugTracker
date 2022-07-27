using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository
{
    public class TicketChangeRepository : GenericRepository<TicketChange>, ITicketChangeRepository
    {

        public TicketChangeRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void CreateTicketChange(Ticket ticket, string userId)
        {
            var entry = _db.Entry(ticket);
            var dbValues = entry.GetDatabaseValues();
            foreach(var property in entry.CurrentValues.Properties)
            {
                var propEntry = entry.Property(property.Name);
                string oldVal = dbValues[property.Name]?.ToString() ?? "N/A";
                string newVal = propEntry.CurrentValue?.ToString() ?? "N/A";
                if (!oldVal.Equals(newVal))
                {
                    TicketChange newChange = new TicketChange()
                    {
                        TicketId = ticket.Id,
                        Property = property.Name,
                        OldValue = oldVal,
                        NewValue = newVal,
                        AuthorId = userId,
                    };
                    _db.TicketChanges.Add(newChange);
                }
            }
        }

        public IEnumerable<TicketChange?> GetForTicket(int ticketId)
        {
            return _db.TicketChanges.Include(c => c.Author).Where(c => c.TicketId == ticketId).ToList();
        }
    }
}
