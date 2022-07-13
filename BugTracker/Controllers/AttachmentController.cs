using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using BugTracker.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class AttachmentController : Controller
    {
        // TODO : security considerations 
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttachmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index(int ticketId)
        {
            var att = new AttachmentViewModel()
            {
                AttachmentList = (List<Attachment>)_unitOfWork.Attachments.GetForTicket(ticketId),
                Attachment = new Attachment
                {
                    TicketId = ticketId
                }

            };
            return PartialView("/Views/Ticket/_AttachmentPartialView.cshtml",att);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttachmentViewModel attachmentData)
        {
            Attachment att = attachmentData.Attachment;
            using (var memoryStream = new MemoryStream())
            {
                await attachmentData.FormFile.CopyToAsync(memoryStream);
                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152 && memoryStream.Length > 0)
                {
                    att.File = memoryStream.ToArray();
                    att.FileName = attachmentData.FormFile.FileName;
                    att.MimeType = attachmentData.FormFile.ContentType;
                    _unitOfWork.Attachments.Add(att);
                    await _unitOfWork.Complete();
                    //create notification
                    var user = await _userManager.GetUserAsync(User);
                    var ticket = _unitOfWork.Tickets.GetById(att.TicketId);
                    var notification = new Notification
                    {
                        Text = $"{user.FirstName} ({user.UserName}) attached a file in ticket: {ticket.Title} ",
                        RefLink = $"/Ticket/Details/{att.TicketId}"
                    };
                    _unitOfWork.Notifications.Create(notification, ticket.ProjectId, user);
                }
          
            }
            return RedirectToAction("Index",new { ticketId = att.TicketId });
        }
        public IActionResult Download(int id)
        {
            var attachment = _unitOfWork.Attachments.GetById(id);
            if(attachment != null)
            {
                return File(attachment.File, attachment.MimeType);
            }
            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var attachmentToDelete = _unitOfWork.Attachments.GetById(id);
            if(attachmentToDelete!= null)
            {
                _unitOfWork.Attachments.Remove(attachmentToDelete);
                await _unitOfWork.Complete();
            }
            return RedirectToAction("Index");
        }
    }
}
