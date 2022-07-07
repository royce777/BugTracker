using BugTracker.Models;

namespace BugTracker.Helpers
{
    public interface INotificationHelper
    {
        List<NotificationApplicationUser> GetUserNotifications(string userId);
        void Create(Notification notification, int projectId);
        void ReadNotification(int notificationId, string userId);
    }
}
