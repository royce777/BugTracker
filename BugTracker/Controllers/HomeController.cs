using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BugTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //private async Task<List<HomeCardSliderViewModel>> GetData(System.Security.Claims.ClaimsPrincipal User)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var data = await _db.Projects.Include(p => p.Tickets).Where(p => p.AssignedUsers.Contains(user)).ToListAsync();
        //    List<HomeCardSliderViewModel> cardsData = new List<HomeCardSliderViewModel>();
        //    foreach(var project in data)
        //    {
        //        HomeCardSliderViewModel cardDataProject = new HomeCardSliderViewModel();
        //        cardDataProject.project = project;
        //        cardsData.Add(cardDataProject);
        //        foreach(var ticket in project.Tickets)
        //        {
        //            HomeCardSliderViewModel cardDataTicket = new HomeCardSliderViewModel();
        //            cardDataTicket.ticket = ticket;
        //            cardsData.Add(cardDataTicket);
        //        }
        //    }
        //    return cardsData;
        //}
        
    }
}