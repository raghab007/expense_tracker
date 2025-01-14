using System.ComponentModel.DataAnnotations;

namespace DotnetCourseowork.Models;


public class Debt
{
    public Debt()
    
    {
    }
    public int Id { get; set; }
    public int Amount { get; set; } 
    public string Description { get; set; }

    [Required(ErrorMessage = "Source is required")]
    public string Source { get; set; }  // Make sure this field is not null or empty
    
    public DateTime? Date { get; set; }
    public bool Paid { get; set; } = false;
}
