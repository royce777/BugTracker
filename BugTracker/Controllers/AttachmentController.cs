using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    public class AttachmentController : Controller
    {
        // TODO : Generate download link in table + security considerations
        private readonly ApplicationDbContext _db;

        public AttachmentController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int ticketId)
        {
            var att = new AttachmentDTO()
            {
                AttachmentList = _db.Attachments.Include(c => c.Author).Where(c => c.TicketId == ticketId).ToList(),
                Attachment = new Attachment
                {
                    TicketId = ticketId
                }

            };
            return PartialView("/Views/Ticket/_AttachmentPartialView.cshtml",att);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttachmentDTO attachmentData)
        {
            //TODO: Add client side validation for empty description or file!
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
                    _db.Attachments.Add(att);
                    _db.SaveChanges();
                }
          
            }
            // return Redirect(Request.Headers["Referer"].ToString());
            return RedirectToAction("Index",new { ticketId = att.TicketId });
        }
        public IActionResult Download(int id)
        {
            var attachment = _db.Attachments.First(att => att.Id == id);
            if(attachment != null)
            {
                return File(attachment.File, attachment.MimeType);
            }
            return NotFound();
        }

        public IActionResult Delete(int id)
        {
            var attachmentToDelete = _db.Attachments.First(c => c.Id == id);
            if(attachmentToDelete!= null)
            {
                _db.Attachments.Remove(attachmentToDelete);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
