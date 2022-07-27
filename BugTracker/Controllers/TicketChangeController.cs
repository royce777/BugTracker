using BugTracker.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    public class TicketChangeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TicketChangeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int ticketId)
        {
            var changes = _unitOfWork.TicketChanges.GetForTicket(ticketId);
            return PartialView("/Views/Ticket/_ChangesPartialView.cshtml", changes);
        }

    }
}
