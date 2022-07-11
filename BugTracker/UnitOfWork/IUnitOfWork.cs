using BugTracker.Repository;

namespace BugTracker.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        ITicketRepository Tickets { get; }
        ICommentRepository Comments { get; }
        IAttachmentRepository Attachments { get; }
        INotificationRepository Notifications { get; }
        IUserRepository Users { get; }
        Task<int> Complete();
    }
}
