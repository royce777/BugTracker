using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Data;
using Microsoft.EntityFrameworkCore;
using BugTracker.UnitOfWork;

namespace BugTracker.Controllers
{
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentController(IUnitOfWork unitOfWork)
        {
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
