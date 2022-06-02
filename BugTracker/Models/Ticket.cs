using System.ComponentModel.DataAnnotations;
using BugTracker.Areas.Identity.Data;

namespace BugTracker.Models;

public enum TicketStatus
{
    Pending,
    [Display(Name ="In Progress")]
    In_Progress,
    Closed,
    Finished
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
    [Display(Name ="Bug Fix")]
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
    public string SubmitterId { get; set; } 
    public virtual ApplicationUser? Submitter { get; set; }
    public string? DeveloperId { get; set; }
    public virtual ApplicationUser? Developer { get; set; }
    [Required]
    public TicketPriority Priority{ get; set; } 
    [Required]
    public TicketStatus Status { get; set; }
    [Required]
    public TicketType Type { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? LastUpdate { get; set; }

    // link to a project
    [Required]
    public int ProjectId { get; set; }
    public virtual Project? Project { get; set; }
    public ICollection<Comment>? Comments { get; set; }

}