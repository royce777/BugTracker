using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Data;
using Microsoft.EntityFrameworkCore;
using BugTracker.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using BugTracker.Areas.Identity.Data;

namespace BugTracker.Controllers
{
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int ticketId)
        {
            var comments = new CommentViewModel()
            {
                CommentList = (List<Comment>)_unitOfWork.Comments.GetForTicket(ticketId),
                Comment = new Comment
                {
                    TicketId = ticketId
                }

            };
            return PartialView("/Views/Ticket/_CommentPartialView.cshtml",comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentViewModel commentData)
        {
            //TODO: Add client side validation for empty comment (now it's server side)!
            Comment com = commentData.Comment;
            if (ModelState.IsValid)
            {
                _unitOfWork.Comments.Add(com);
                await _unitOfWork.Complete();
                //create notification
                var user = await _userManager.GetUserAsync(User);
                var ticket = _unitOfWork.Tickets.GetById(com.TicketId);
                var notification = new Notification
                {
                    Text = $"{user.FirstName} ({user.UserName}) commented in ticket: {ticket.Title} ",
                    RefLink = $"/Ticket/Details/{com.TicketId}"
                };
                _unitOfWork.Notifications.Create(notification, ticket.ProjectId, user);
            }
            // return Redirect(Request.Headers["Referer"].ToString());
            return RedirectToAction("Index",new { ticketId = com.TicketId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var commentToDelete = _unitOfWork.Comments.GetById(id);
            if(commentToDelete != null)
            {
                _unitOfWork.Comments.Remove(commentToDelete);
                await _unitOfWork.Complete();
            }
            return RedirectToAction("Index");
        }
    }
}
