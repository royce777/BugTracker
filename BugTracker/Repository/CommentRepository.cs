using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext db) : base(db)
        {
        }

        public IEnumerable<Comment> GetForTicket(int ticketId)
        {
            return _db.Comments.Include(c => c.Author).Where(c => c.TicketId == ticketId).ToList();
        }
    }
}
