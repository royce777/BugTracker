using BugTracker.Models;

namespace BugTracker.Repository
{
    public interface INotificationRepository
    {
        List<NotificationApplicationUser> GetAllUserNotifications(string userId);
        List<NotificationApplicationUser> GetUserNotifications(string userId);
        void Create(Notification notification, int projectId);
        void ReadNotification(int notificationId, string userId);
    }
}
