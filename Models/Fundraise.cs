using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Fundraise(string title, string description, decimal goalAmount, DateTime startDate, DateTime endDate)
{
    [Key]
    [MaxLength(50)]
    public string FundraiseId { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = title;
    
    [Required]
    public string Description { get; set; } = description;
    
    [Required]
    public decimal GoalAmount { get; set; } = goalAmount;
    
    [Required]
    public DateTime StartDate { get; set; } = startDate;
    
    [Required]
    public DateTime EndDate { get; set; } = endDate;

    [Required]
    [MaxLength(50)]
    [ForeignKey("Class")]
    public string? ClassId { get; set; }
    public virtual Class? Class { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("Account")]
    public string? AccountNumber { get; set; }
    public virtual Account? Account { get; set; }

    public virtual ICollection<Transaction>? Transactions { get; set; } = [];
}