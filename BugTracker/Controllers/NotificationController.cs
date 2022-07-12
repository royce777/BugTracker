using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using BugTracker.Repository;
using BugTracker.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> GetAllUserNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            List<NotificationApplicationUser> notifications = _unitOfWork.Notifications.GetAllUserNotifications(user.Id);
            return View("UserNotifications",notifications);
        }

        public IActionResult GetNotifications()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = _unitOfWork.Notifications.GetUserNotifications(userId);
            return Ok(new { UserNotifications = notifications, Count = notifications.Count });
        }

        public IActionResult ReadNotification(int notificationId)
        {
            var userId = _userManager.GetUserId(User);
            _unitOfWork.Notifications.ReadNotification(notificationId, userId);
            return Ok();
        }

    }
}
