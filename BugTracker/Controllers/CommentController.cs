using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BugTracker.Data;

namespace BugTracker.Controllers
{
    public class CommentController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Comment obj)
        {
            if (ModelState.IsValid)
            {
                _db.Comments.Add(obj);
                _db.SaveChanges();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(obj);
        }
    }
}
