using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                IEnumerable<Project> objProjects = _db.Projects;
                return View(objProjects);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                IEnumerable<Project> objProjects = _db.Projects.Where(p => p.AssignedUsers.Contains(user));
                return View(objProjects);
            }
        }

        //GET
        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Create(Project obj)
        {
            var user = await _userManager.GetUserAsync(User);
            obj.AssignedUsers = new List<ApplicationUser>();
            obj.AssignedUsers.Add(user);
            if (ModelState.IsValid)
            {
                _db.Projects.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult Edit(int? id)
        {
           if(id == null || id == 0)
            {
                return NotFound();
            }
            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult Edit(Project obj)
        {
            if (ModelState.IsValid)
            {
                _db.Projects.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult Delete(int? id)
        {
           if(id == null || id == 0)
            {
                return NotFound();
            }
            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]
        public IActionResult DeletePOST(int? id)
        {
            var project = _db.Projects.Find(id);
            if(project == null)
            {
                return NotFound();
            }
            _db.Projects.Remove(project);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            var project = _db.Projects.Include(p => p.Tickets).Include(p => p.AssignedUsers).FirstOrDefault( p => p.Id == id);
            var user = await _userManager.GetUserAsync(User);
            if(project == null) {
                return NotFound();
            }
            if (!project.AssignedUsers.Contains(user))
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

    }
}
