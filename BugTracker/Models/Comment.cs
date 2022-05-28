using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models;

public class Comment
{
    public int Id { get; set; }
    public ApplicationUser Author { get; set; }
    public Ticket Ticket { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
}