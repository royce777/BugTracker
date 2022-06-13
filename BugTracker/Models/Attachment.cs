using BugTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public int TicketId { get; set; }   
        public virtual Ticket? Ticket { get; set; }
        public string AuthorId {get; set;}
        public virtual ApplicationUser? Author { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
