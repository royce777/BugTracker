using BugTracker.Data;
using BugTracker.Hubs;
using BugTracker.Repository;
using Microsoft.AspNetCore.SignalR;

namespace BugTracker.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hub;
        public UnitOfWork(ApplicationDbContext db, IHubContext<NotificationHub> hub)
        {
            _db = db;
            _hub = hub; 
            Projects = new ProjectRepository(_db);
            Tickets = new TicketRepository(_db);
            Comments = new CommentRepository(_db);
            Attachments = new AttachmentRepository(_db);
            Notifications = new NotificationRepository(_db, _hub);
            Users = new UserRepository(_db);
            TicketChanges = new TicketChangeRepository(_db);
        }

        public IProjectRepository Projects { get; private set; }

        public ITicketRepository Tickets { get; private set; }

        public ICommentRepository Comments { get; private set; }

        public IAttachmentRepository Attachments { get; private set; }

        public INotificationRepository Notifications { get; private set; }

        public IUserRepository Users { get; private set; }
        public ITicketChangeRepository TicketChanges { get; private set; }

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
