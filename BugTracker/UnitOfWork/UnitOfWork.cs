using BugTracker.Data;
using BugTracker.Repository;

namespace BugTracker.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Projects = new ProjectRepository(_db);
            Tickets = new TicketRepository(_db);
            Comments = new CommentRepository(_db);
            Attachments = new AttachmentRepository(_db);
            Notifications = new NotificationRepository(_db);
            Users = new UserRepository(_db);
        }

        public IProjectRepository Projects { get; private set; }

        public ITicketRepository Tickets { get; private set; }

        public ICommentRepository Comments { get; private set; }

        public IAttachmentRepository Attachments { get; private set; }

        public INotificationRepository Notifications { get; private set; }

        public IUserRepository Users { get; private set; }

        public Task<int> Complete()
        {
            return _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
