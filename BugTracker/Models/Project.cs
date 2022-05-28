using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;


}