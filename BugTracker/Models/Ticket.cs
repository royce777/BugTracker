using System.ComponentModel.DataAnnotations;
using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models;

public enum TicketStatus
{
    Pending,
    In_Progress,
    Closed
}

public enum TicketPriority
{
    Low,
    Intermediate,
    Urgent,
    Maximum
}

public enum TicketType
{
    BugFix,
    Improve,
    Feature
}

public class Ticket
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public ApplicationUser Submitter { get; set; }
    [Required]
    public ApplicationUser Developer { get; set; }
    [Required]
    public TicketPriority Priority{ get; set; } 
    [Required]
    public TicketStatus Status { get; set; }
    [Required]
    public TicketType Type { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    [Required]
    public DateTime LastUpdate { get; set; }
    
    // link to a project
    public Project Project { get; set; }

}