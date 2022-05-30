using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
