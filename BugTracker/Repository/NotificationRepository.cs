using BugTracker.Data;
using BugTracker.Hubs;
using BugTracker.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hub;
        public NotificationRepository(ApplicationDbContext db, IHubContext<NotificationHub> hub)
        {
            _db = db;
            _hub = hub;
        }
        public List<NotificationApplicationUser> GetAllUserNotifications(string userId)
        {
            return _db.UserNotifications.Include( n => n.Notification ).Where( n => n.UserId == userId ).ToList();
            
        }

        public void Create(Notification notification, int projectId)
        {
            var project = _db.Projects.Include(p => p.AssignedUsers).FirstOrDefault(p => p.Id == projectId);
            var projectUsers = project.AssignedUsers;
            _db.Notifications.Add(notification);
            _db.SaveChanges();

            foreach(var user in projectUsers)
            {
                var userNotification = new NotificationApplicationUser
                {
                    NotificationId = notification.Id,
                    UserId = user.Id
                };
                _db.UserNotifications.Add(userNotification);
                _db.SaveChanges();
            }
            _hub.Clients.All.SendAsync("getNotifications", "");
            
        }
        public List<NotificationApplicationUser> GetUserNotifications(string userId)
        {
            return _db.UserNotifications.Where(u => u.UserId.Equals(userId) && !u.Read)
                                            .Include(n => n.Notification)
                                            .ToList();
        }

        public void ReadNotification(int notificationId, string userId)
        {
            var notification = _db.UserNotifications
                                        .FirstOrDefault(n => n.UserId.Equals(userId)
                                        && n.NotificationId == notificationId);
            notification.Read = true;
            _db.UserNotifications.Update(notification);
            _db.SaveChanges();
        }
    }
}
