using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Fundraise
{
    [Key]
    [MaxLength(50)]
    public string FundraiseId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }

    [Required] 
    public decimal GoalAmount { get; set; }

    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("Class")]
    public string ClassId { get; set; }
    public virtual Class? Class { get; set; }

    [Required]
    [MaxLength(12)]
    [ForeignKey("Account")]
    public string AccountNumber { get; set; }
    public virtual Account? Account { get; set; }

    public Fundraise()
    {
        FundraiseId = Guid.NewGuid().ToString();
        Title ??= string.Empty;
        Description ??= string.Empty;
        ClassId ??= string.Empty;
        AccountNumber ??= string.Empty;
    }
}