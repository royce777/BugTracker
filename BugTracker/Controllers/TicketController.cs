using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }   

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(ticket => ticket.Project);
                return View(tickets);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                IEnumerable<Ticket> tickets = _db.Tickets.Include(ticket => ticket.Project).Where(ticket => ticket.Project.AssignedUsers.Contains(user));
                return View(tickets);
            }
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            //retrieve projects from DB
            var projects = _db.Projects.ToList();
            var users = _db.Users.ToList();
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = users;
            return View();
        }
        public async Task<IActionResult> CreateFromProject(int projectId)
        {
            // check is the user is in this project
            var project = await _db.Projects.Include(p => p.AssignedUsers).FirstOrDefaultAsync(p => p.Id == projectId);
            if(project == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!project.AssignedUsers.Contains(user))
            {
                ViewBag.ErrorTitle = "Unable to create new ticket.";
                ViewBag.ErrorMessage = "You are not a member of this project.";
                return View("Error");
            }
            ViewBag.DisableProject = true;
            ViewBag.ProjectName = project.Title;
            var projects = _db.Projects.ToList();
            var users = _db.Users.ToList();
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = users;
            return View("Create");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket obj)
        {

            if (ModelState.IsValid)
            {
                _db.Tickets.Add(obj);
                _db.SaveChanges();
                //return Redirect(Request.Headers["Referer"].ToString()); 
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
        
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            var ticket = await _db.Tickets.Include(t => t.Project.AssignedUsers).FirstOrDefaultAsync(t=> t.Id == id);
            var user = await _userManager.GetUserAsync(User);
            if (ticket == null)
            {
                return NotFound();
            }
            if (!ticket.Project.AssignedUsers.Contains(user) && !User.IsInRole("Admin") )
            {
                ViewBag.ErrorTitle = "Ticket " + ticket.Title + " cannot be edited.";
                ViewBag.ErrorMessage = "You have no privileges to complete this operation.";
                return View("Error");
            }
            if (User.IsInRole("Admin"))
            {
                ViewBag.Admin = true;
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
