using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Dynamic;

namespace BugTracker.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProjectController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Project> objProjects = _db.Projects;
            return View(objProjects);
        }

        //GET
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project obj)
        {
            if (ModelState.IsValid)
            {
                _db.Projects.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
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

        public IActionResult Details(int? id)
        {
            var project = _db.Projects.Include(p => p.Tickets).Include(p => p.AssignedUsers).FirstOrDefault( p => p.Id == id);
            if(project == null) {
                return NotFound();
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

            }

            // CHART JS
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
            return View(project);
        }

    }
}
