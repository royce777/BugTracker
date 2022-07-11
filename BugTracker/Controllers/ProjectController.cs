using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Repository;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BugTracker.UnitOfWork;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                IEnumerable<Project> objProjects = _unitOfWork.Projects.GetAll();
                return View(objProjects);
            }
            else if (User.IsInRole("Demo"))
            {
                IEnumerable<Project> objProjects = _unitOfWork.Projects.Find(p => p.Demo == true);
                return View(objProjects);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                IEnumerable<Project> objProjects = _unitOfWork.Projects.Find(p=> p.AssignedUsers.Contains(user)); 
                return View(objProjects);
            }
        }

        //GET
        [Authorize(Roles = "Admin,Project Manager,Demo")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager,Demo")]
        public async Task<IActionResult> Create(Project obj)
        {
            var user = await _userManager.GetUserAsync(User);
            obj.AssignedUsers = new List<ApplicationUser>();
            obj.AssignedUsers.Add(user);
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Demo"))
                {
                    obj.Demo = true;
                }
                _unitOfWork.Projects.Add(obj);
                await _unitOfWork.Complete();
                TempData["success"] = "Project succesfully created";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Control provided data and try again !";
            return View(obj);
        }

        // GET
        [Authorize(Roles = "Admin,Project Manager,Demo")]
        public IActionResult Edit(int id)
        {
           if(id == 0)
            {
                return NotFound();
            }
            var project = _unitOfWork.Projects.GetById(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager,Demo")]
        public async Task<IActionResult> Edit(Project obj)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Demo") && !obj.Demo)
                {
                    ViewBag.ErrorTitle = "Unable to edit project details.";
                    ViewBag.ErrorMessage = "You can modify only demo projects !";
                    return View("Error");
                }
                _unitOfWork.Projects.Update(obj);
                await _unitOfWork.Complete();
                TempData["success"] = "Project information updated !";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Check provided info and try again !";
            return View(obj);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult Delete(int id)
        {
           if(id == 0)
            {
                return NotFound();
            }
            var project = _unitOfWork.Projects.GetById(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> DeletePOST(int id)
        {
            var project = _unitOfWork.Projects.GetById(id);
            if(project == null)
            {
                return NotFound();
            }
            _unitOfWork.Projects.Remove(project);
            await _unitOfWork.Complete();
            TempData["success"] = "Project successfully deleted !";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _unitOfWork.Projects.GetDetails(id);
            var user = await _userManager.GetUserAsync(User);
            if(User.IsInRole("Demo") && !project.Demo)
            {
                ViewBag.ErrorTitle = "Unable to view project details.";
                ViewBag.ErrorMessage = "You are not a member of this project !";
                return View("Error");
            }
            if(project == null) {
                return NotFound();
            }
            if (!project.AssignedUsers.Contains(user) && !User.IsInRole("Admin"))
            {
                ViewBag.ErrorTitle = "Unable to view project details.";
                ViewBag.ErrorMessage = "You are not a member of this project !";
                return View("Error");
            }
            //priority counters
            int low = 0;
            int intermediate = 0;
            int urgent = 0;
            int maximum = 0;
            // status counters
            int pending = 0;
            int inProgress = 0;
            int closed = 0;
            int finished = 0;
            // type counters
            int bugFix = 0;
            int improve = 0;
            int feature = 0;

            int total = 0;
            foreach(Ticket t in project.Tickets)
            {
                total++;
                switch (t.Priority) 
                {
                    case TicketPriority.Low:
                        {
                            low++;  break;
                        }
                    case TicketPriority.Intermediate:
                        {
                            intermediate++; break;
                        }
                    case TicketPriority.Urgent:
                        {
                            urgent++; break;
                        }
                    case TicketPriority.Maximum:
                        {
                            maximum++; break;
                        }
                }
                switch(t.Status)
                {
                    case TicketStatus.Pending:
                        {
                            pending++; break;
                        }
                    case TicketStatus.In_Progress:
                        {
                            inProgress++; break;
                        }
                    case TicketStatus.Closed:
                        {
                            closed++; break;
                        }
                    case TicketStatus.Finished:
                        {
                            finished++; break;
                        }
                }
                switch (t.Type)
                {
                    case TicketType.BugFix:
                        {
                            bugFix++; break;
                        }
                    case TicketType.Improve:
                        {
                            improve++; break;
                        }
                    case TicketType.Feature:
                        {
                            feature++; break;
                        }
                }

            }
            if (total > 0)
            {
                // CHART JS priority
                string[] labels = new string[] { "Low", "Intermediate", "Urgent", "Maximum" };
                int[] dataArr = new int[] { low * 100 / total, intermediate * 100 / total, urgent * 100 / total, maximum * 100 / total };
                string[] backgroundColor = new string[] { "rgb(51, 153, 255)", "rgb(0, 255, 204)", "rgb(204, 153, 255)", "rgb(255, 204, 153)" };
                Chart.Data.Datasets datasets = new Chart.Data.Datasets();
                datasets.data = dataArr;
                datasets.label = "Ticket Priority";
                datasets.backgroundColor = backgroundColor;
                Chart.Data data = new Chart.Data();
                data.labels = labels;
                data.datasets = new List<Chart.Data.Datasets>();
                data.datasets.Add(datasets);
                Chart chart = new Chart();
                chart.type = "pie";
                chart.responsive = true;
                chart.data = data;
                ViewBag.priorityData = JsonConvert.SerializeObject(chart);

                //CHART JS status
                labels = new string[] { "Pending", "In Progress", "Closed", "Finished" };
                dataArr = new int[] { pending * 100 / total, inProgress * 100 / total, closed * 100 / total, finished * 100 / total };
                // keep same colors
                datasets.data = dataArr;
                data.labels = labels;
                datasets.label = "Ticket Status";
                ViewBag.statusData = JsonConvert.SerializeObject(chart);

                //CHART JS type
                labels = new string[] { "Bug Fix", "Improvement", "Feature" };
                dataArr = new int[] { bugFix * 100 / total, improve * 100 / total, feature * 100 / total };
                // keep same colors
                datasets.data = dataArr;
                data.labels = labels;
                datasets.label = "Ticket Type";
                ViewBag.typeData = JsonConvert.SerializeObject(chart);
                ViewBag.showChart = "Yes";
            }
            else
            {
                ViewBag.showChart = "No";
            }

            return View(project);
        }

        public async Task<IActionResult> ManageProjectUsers(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var project = await _unitOfWork.Projects.GetWithUsers(id);
            if(project == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            bool demouser = User.IsInRole("Demo");
            if(!(project.AssignedUsers.Contains(user) && User.IsInRole("Project Manager")) && !User.IsInRole("Admin"))
            {
                if(!(demouser && project.Demo))
                {
                    ViewBag.ErrorTitle = "You cannot modify the team of this project.";
                    ViewBag.ErrorMessage = "Please verify you have the necessary privileges to complete this operation \n" +
                        "NOTE : Only Admins and Project Managers can modify users assigned to the project.";
                    return View("Error");
                }
            }
            ManageProjectUsersViewModel model = new ManageProjectUsersViewModel
            {
                Id = project.Id,
                ProjectName = project.Title
            };
            foreach(var usr in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(usr);
                if( (demouser && roles.Contains("Demo")) || (!demouser && !roles.Contains("Demo")) )
                {
                    if (project.AssignedUsers.Contains(usr))
                    {
                        model.Members.Add(new UserRoles { User = usr, Roles = roles.ToList() });
                    }
                    else
                    {
                        model.Others.Add(new UserRoles { User = usr, Roles = roles.ToList() });
                    }
                }
            }
            return View(model);


        }

        [HttpPost]
        public async Task<IActionResult> AddUsers(string[] userNames, int projectId)
        {
            if(projectId== 0)
            {
                return NotFound();
            }
            var project = await _unitOfWork.Projects.GetWithUsers(projectId);
            if(project == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            bool demouser = User.IsInRole("Demo");
            if(!(project.AssignedUsers.Contains(user) && User.IsInRole("Project Manager")) && !User.IsInRole("Admin"))
            {
                if(!(demouser && project.Demo))
                {
                    // generate some error to display
                    ViewBag.ErrorTitle = "You cannot modify the team of this project.";
                    ViewBag.ErrorMessage = "Please verify you have the necessary privileges to complete this operation \n" +
                        "NOTE : Only Admins and Project Managers can modify users assigned to the project.";
                    return View("Error");
                }
            }
            if(userNames.Length < 1)
            {
            }
            foreach(string uname in userNames)
            {
                var usr = await _userManager.FindByNameAsync(uname);
                if (usr == null) return NotFound();
                var roles = await _userManager.GetRolesAsync(usr);
                if ((demouser && roles.Contains("Demo")) || (!demouser && !roles.Contains("Demo")))
                {
                    project.AssignedUsers.Add(usr);
                }
            }
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.Complete();
            //create notification
            var notification = new Notification
            {
                Text = $"{project.Title} staff has changed !",
                RefLink = $"/Project/Details/{project.Id}"
            };
            _unitOfWork.Notifications.Create(notification, projectId);
            
            return RedirectToAction("ManageProjectUsers", new { id = project.Id });

        }

        [HttpPost]
        public async Task<IActionResult> RemoveUsers(string[] userNames, int projectId)
        {
            if(projectId== 0)
            {
                return NotFound();
            }
            var project = await _unitOfWork.Projects.GetWithUsers(projectId);
            if(project == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            bool demouser = User.IsInRole("Demo");
            if(!(project.AssignedUsers.Contains(user) && User.IsInRole("Project Manager")) && !User.IsInRole("Admin"))
            {
                if(!(demouser && project.Demo))
                {
                    // generate some error to display
                    ViewBag.ErrorTitle = "You cannot modify the team of this project.";
                    ViewBag.ErrorMessage = "Please verify you have the necessary privileges to complete this operation \n" +
                        "NOTE : Only Admins and Project Managers can modify users assigned to the project.";
                    return View("Error");
                }
            }
            if(userNames.Length < 1)
            {
            }
            foreach(string uname in userNames)
            {
                var usr = await _userManager.FindByNameAsync(uname);
                if (usr == null) return NotFound();
                var roles = await _userManager.GetRolesAsync(usr);
                if ((demouser && roles.Contains("Demo")) || (!demouser && !roles.Contains("Demo")))
                {
                    project.AssignedUsers.Remove(usr);
                }
            }
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.Complete();
            return RedirectToAction("ManageProjectUsers", new { id = project.Id });

        }

    }
}
