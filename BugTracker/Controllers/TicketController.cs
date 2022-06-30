using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                TempData["success"] = "Ticket successfully created ! ";
                if (Request.Headers["Referer"].ToString().Contains("CreateFromProject"))
                {
                    return RedirectToAction("Details", "Project", new { id = obj.ProjectId });
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to create the ticket, check information and try again !";
            return View(obj);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var ticket = await _db.Tickets.Include(t=> t.Project.AssignedUsers).Include(t => t.Submitter).Include(t=> t.Developer).FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();

            }
            // Restrict users out of project to access the ticket
            if(!ticket.Project.AssignedUsers.Contains(user) && !User.IsInRole("Admin"))
            {
                ViewBag.ErrorTitle = "Unable to access ticket " + ticket.Title + " !";
                ViewBag.ErrorMessage = "You don't have the privileges to access this resource !";
                return View("Error");
            }
            if(User.IsInRole("Developer") && ticket.Status == TicketStatus.Pending)
            {
                ViewBag.ActionButton = "Start";
            }
            else if(User.IsInRole("Developer") && ticket.Status == TicketStatus.In_Progress)
            {
                ViewBag.ActionButton = "Finish";
            }
            else if(User.IsInRole("Project Manager") && ticket.Status == TicketStatus.Finished)
            {
                ViewBag.ActionButton = "Close";
            }
            return View(ticket);    
        }
        
        [Authorize(Roles = "Admin,Project Manager")]
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
        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult Edit(Ticket obj)
        {
            if (ModelState.IsValid)
            {
                _db.Tickets.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Ticket successfully updated !";
                return RedirectToAction("Details", new {id = obj.Id});
            }
            TempData["error"] = "Unable to edit ticket, check information and try again !";
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
            TempData["success"] = "Ticket successfully deleted !";
            return Redirect(Request.Headers["Referer"].ToString()); 
        }

        [HttpPost]
        public async Task<IActionResult> WorkflowAction(string action, int ticketId)
        {
            if (ticketId == 0)
            {
                return NotFound();
            }
            var ticket = await _db.Tickets.Include(t => t.Project.AssignedUsers).FirstOrDefaultAsync( t => t.Id == ticketId);
            if(ticket == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!ticket.Project.AssignedUsers.Contains(user))
            {
                ViewBag.ErrorTitle = "Ticket " + ticket.Title + " cannot be edited.";
                ViewBag.ErrorMessage = "You have no privileges to complete this operation.";
                return View("Error");
            }
            bool success = false;
            string newStatus = "";
            string newAction = "";
            switch (action)
            {
                case "Start":
                    if (User.IsInRole("Developer") && ticket.Status== TicketStatus.Pending)
                    {
                        ticket.Status = TicketStatus.In_Progress;
                        _db.Tickets.Update(ticket);
                        await _db.SaveChangesAsync();
                        newStatus = "In Progress";
                        newAction = "Finish";
                        success = true;
                    }
                    break;
                case "Finish":
                    if(User.IsInRole("Developer") && ticket.Status == TicketStatus.In_Progress)
                    {
                        ticket.Status = TicketStatus.Finished;
                        _db.Tickets.Update(ticket);
                        await _db.SaveChangesAsync();
                        newStatus = "Finished";
                        if(User.IsInRole("Project Manager") || User.IsInRole("Architect"))
                        {
                            newAction = "Close";
                        }
                        else
                        {
                            newAction = "";
                        }
                        success = true;
                    }
                    break;
                case "Close":
                    if((User.IsInRole("Architect") || User.IsInRole("Project Manager")) && ticket.Status == TicketStatus.Finished)
                    {
                        ticket.Status = TicketStatus.Closed;
                        _db.Tickets.Update(ticket);
                        await _db.SaveChangesAsync();
                        newStatus = "Closed";
                        success = true;
                    }
                    break;
            }
            if (success)
            {
                var result = new
                {
                    action = newAction,
                    status = newStatus,
                };
                return Ok(result);
            }
            else return BadRequest();
        }
    }
}
