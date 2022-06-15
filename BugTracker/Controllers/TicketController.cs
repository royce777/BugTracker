using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    [Authorize]
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

        public IActionResult Details(int? id)
        {
            var ticket = _db.Tickets.Include(t => t.Submitter).Include(t=> t.Developer).FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();

            }
            return View(ticket);    
        }
        
        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            var ticket = _db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var projects = _db.Projects.ToList();
            var users = _db.Users.ToList();
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = users;
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Ticket obj)
        {
            if (ModelState.IsValid)
            {
                _db.Tickets.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            var ticket = _db.Tickets.Find(id);
            if(ticket == null)
            {
                return NotFound();
            }
            _db.Tickets.Remove(ticket);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString()); 
        }
    }
}
