using BugTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Comment
{
    public int Id { get; set; }
    public string AuthorId {get; set;}
    public virtual ApplicationUser? Author { get; set; }
    public int TicketId { get; set; }   
    public virtual Ticket? Ticket { get; set; }
    [Required(ErrorMessage = "Please type in your comment !")]
    public string Message { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}