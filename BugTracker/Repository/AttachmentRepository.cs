using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository
{
    public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(ApplicationDbContext db) : base(db)
        {
        }

        public IEnumerable<Attachment> GetForTicket(int ticketId)
        {
            return _db.Attachments.Include(c => c.Author).Where(c => c.TicketId == ticketId).ToList();
        }
    }
}
