using BugTracker.Models;

namespace BugTracker.Repository
{
    public interface IAttachmentRepository : IGenericRepository<Attachment>
    {
        IEnumerable<Attachment> GetForTicket(int ticketId);
    }
}
