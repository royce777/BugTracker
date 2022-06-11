using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BugTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    public class CommentController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CommentController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int ticketId)
        {
            var comments = new CommentDTO()
            {
                CommentList = _db.Comments.Include(c => c.Author).Where(c => c.TicketId == ticketId).ToList(),
                Comment = new Comment
                {
                    TicketId = ticketId
                }

            };
            return PartialView("/Views/Ticket/_CommentPartialView.cshtml",comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CommentDTO commentData)
        {
            //TODO: Add client side validation for empty comment !
            Comment com = commentData.Comment;
            _db.Comments.Add(com);
            _db.SaveChanges();
            // return Redirect(Request.Headers["Referer"].ToString());
            return RedirectToAction("Index",new { ticketId = com.TicketId });
        }

        public IActionResult Delete(int id)
        {
            var commentToDelete = _db.Comments.First(c => c.Id == id);
            if(commentToDelete != null)
            {
                _db.Comments.Remove(commentToDelete);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
