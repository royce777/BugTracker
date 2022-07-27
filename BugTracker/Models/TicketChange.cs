using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models
{
    public class TicketChange
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }
        public string? Property { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;


    }
}
