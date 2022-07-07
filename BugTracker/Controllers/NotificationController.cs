using BugTracker.Areas.Identity.Data;
using BugTracker.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController(INotificationRepository notificationRepository, UserManager<ApplicationUser> userManager)
        {
            _notificationRepository = notificationRepository;
            _userManager = userManager;
        }

        public IActionResult GetNotifications()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = _notificationRepository.GetUserNotifications(userId);
            return Ok(new { UserNotifications = notifications, Count = notifications.Count });
        }

        public IActionResult ReadNotification(int notificationId)
        {
            var userId = _userManager.GetUserId(User);
            _notificationRepository.ReadNotification(notificationId, userId);
            return Ok();
        }

    }
}
