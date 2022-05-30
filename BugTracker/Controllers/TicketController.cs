using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TicketController(ApplicationDbContext db)
        {
            _db = db;
        }   

        public IActionResult Index()
        {
            IEnumerable<Ticket> tickets = _db.Tickets.Include(ticket => ticket.Project);
            return View(tickets);
        }
        public IActionResult Create()
        {
            //retrieve projects from DB
            var projects = _db.Projects.ToList();
            var users = _db.Users.ToList();
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = users;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket obj)
        {

            if (ModelState.IsValid)
            {
                _db.Tickets.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    }
}
