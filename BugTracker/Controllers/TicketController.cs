using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }   

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                IEnumerable<Ticket> tickets = _unitOfWork.Tickets.GetAll();
                return View(tickets);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                IEnumerable<Ticket> tickets = _unitOfWork.Tickets.GetForUser(user);
                return View(tickets);
            }
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            //retrieve projects from DB
            var projects = _unitOfWork.Projects.GetAll();
            var users = _unitOfWork.Users.GetAll();
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = users;
            return View();
        }
        public async Task<IActionResult> CreateFromProject(int projectId)
        {
            // check is the user is in this project
            var project = await _unitOfWork.Projects.GetWithUsers(projectId);
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
            var projects = _unitOfWork.Projects.GetAll();
            List<ApplicationUser> compatibleUsers = new List<ApplicationUser>();
            List<ApplicationUser> allUsers = (List<ApplicationUser>)_unitOfWork.Users.GetAll();
            if (project.Demo)
            {
                foreach(var usr in allUsers)
                {
                    if ( await _userManager.IsInRoleAsync(usr, "Demo"))
                    {
                        compatibleUsers.Add(usr);
                    }
                }
            }
            else
            {
                foreach(var usr in allUsers)
                {
                    if(! await _userManager.IsInRoleAsync(usr, "Demo"))
                    {
                        compatibleUsers.Add(usr);
                    }
                }
            }
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = compatibleUsers;
            return View("Create");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket obj)
        {

            if (ModelState.IsValid)
            {
                obj.Created = DateTime.Now;
                _unitOfWork.Tickets.Add(obj);
                await _unitOfWork.Complete();
                //return Redirect(Request.Headers["Referer"].ToString()); 
                TempData["success"] = "Ticket successfully created ! ";
                //create notification
                var user = await _userManager.GetUserAsync(User);
                var project = _unitOfWork.Projects.GetById(obj.ProjectId);
                var notification = new Notification
                {
                    Text = $"{project.Title} has new ticket : {obj.Title}!",
                    RefLink = $"/Project/Details/{project.Id}"
                };
                _unitOfWork.Notifications.Create(notification, obj.ProjectId, user);

                if (Request.Headers["Referer"].ToString().Contains("CreateFromProject"))
                {
                    return RedirectToAction("Details", "Project", new { id = obj.ProjectId });
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to create the ticket, check information and try again !";
            return View(obj);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var ticket = await _unitOfWork.Tickets.GetDetails(id);
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
            if( (User.IsInRole("Developer") || User.IsInRole("Demo")) && ticket.Status == TicketStatus.Pending)
            {
                ViewBag.ActionButton = "Start";
            }
            else if( (User.IsInRole("Developer") || User.IsInRole("Demo")) && ticket.Status == TicketStatus.In_Progress)
            {
                ViewBag.ActionButton = "Finish";
            }
            else if( (User.IsInRole("Project Manager") || User.IsInRole("Demo")) && ticket.Status == TicketStatus.Finished)
            {
                ViewBag.ActionButton = "Close";
            }
            return View(ticket);    
        }
        
        [Authorize(Roles = "Admin,Project Manager,Demo")]
        public async Task<IActionResult> Edit(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var ticket = await _unitOfWork.Tickets.GetWithUsers(id);
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
            var projects = _unitOfWork.Projects.GetAll();
            var users = _unitOfWork.Users.GetAll();
            ViewData["ProjectsList"] = projects;
            ViewData["Users"] = users;
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager,Demo")]
        public async Task<IActionResult> Edit(Ticket obj)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _unitOfWork.Projects.GetWithUsers(obj.ProjectId);
            if(project == null)
            {
                return NotFound();
            }
            if (!project.AssignedUsers.Contains(user))
            {
                ViewBag.ErrorTitle = "Ticket " + obj.Title + " cannot be edited.";
                ViewBag.ErrorMessage = "You have no privileges to complete this operation.";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Tickets.Update(obj);
                // Track properties for changement history
                _unitOfWork.TicketChanges.CreateTicketChange(obj, user.Id);
                await _unitOfWork.Complete();
                //create notification
                var notification = new Notification
                {
                    Text = $"{user.FirstName} ({user.UserName}) has edited ticket : {obj.Title} ",
                    RefLink = $"/Ticket/Details/{obj.Id}"
                };
                _unitOfWork.Notifications.Create(notification, obj.ProjectId, user);

                TempData["success"] = "Ticket successfully updated !";
                return RedirectToAction("Details", new {id = obj.Id});
            }
            TempData["error"] = "Unable to edit ticket, check information and try again !";
            return View(obj);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var ticket = _unitOfWork.Tickets.GetById(id);
            if(ticket == null)
            {
                return NotFound();
            }
            _unitOfWork.Tickets.Remove(ticket);
            await _unitOfWork.Complete();
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
            var ticket = await _unitOfWork.Tickets.GetWithUsers(ticketId);
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
                    if ( (User.IsInRole("Developer") || User.IsInRole("Demo")) && ticket.Status== TicketStatus.Pending)
                    {
                        ticket.Status = TicketStatus.In_Progress;
                        _unitOfWork.Tickets.Update(ticket);
                        _unitOfWork.TicketChanges.CreateTicketChange(ticket, user.Id);
                        await _unitOfWork.Complete();
                        newStatus = "In Progress";
                        newAction = "Finish";
                        success = true;
                    }
                    break;
                case "Finish":
                    if( (User.IsInRole("Developer") || User.IsInRole("Demo")) && ticket.Status == TicketStatus.In_Progress)
                    {
                        ticket.Status = TicketStatus.Finished;
                        _unitOfWork.Tickets.Update(ticket);
                        _unitOfWork.TicketChanges.CreateTicketChange(ticket, user.Id);
                        await _unitOfWork.Complete();
                        newStatus = "Finished";
                        if(User.IsInRole("Project Manager") || User.IsInRole("Architect")|| User.IsInRole("Demo"))
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
                    if((User.IsInRole("Architect") || User.IsInRole("Project Manager")|| User.IsInRole("Demo")) && ticket.Status == TicketStatus.Finished)
                    {
                        ticket.Status = TicketStatus.Closed;
                        _unitOfWork.Tickets.Update(ticket);
                        _unitOfWork.TicketChanges.CreateTicketChange(ticket, user.Id);
                        await _unitOfWork.Complete();
                        newStatus = "Closed";
                        success = true;
                    }
                    break;
            }
            if (success)
            {
                //create notification
                var notification = new Notification
                {
                    Text = $@"Ticket : ""{ticket.Title}"" has new status : {newStatus}. By {user.FirstName} ({user.UserName}) ",
                    RefLink = $"/Ticket/Details/{ticket.Id}"
                };
                _unitOfWork.Notifications.Create(notification, ticket.ProjectId, user);

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
