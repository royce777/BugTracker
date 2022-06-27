using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.ViewComponents
{
    public class HomeCardSlider : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeCardSlider(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
    
        public async Task<IViewComponentResult> InvokeAsync(string type)
        {
            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            var data = await _db.Projects.Include(p => p.Tickets).Where(p => p.AssignedUsers.Contains(user)).ToListAsync();
            HomeCardSliderViewModel model = new HomeCardSliderViewModel();
            if(type == "project")
            {
                model.projects = data;
            }
            else if(type == "ticket")
            {
                model.tickets = new List<Ticket>();
                foreach(var project in data)
                {
                    foreach(var ticket in project.Tickets)
                    {
                        model.tickets.Add(ticket);
                    }
                }
            }
            return View(model);
        }

    }
}
