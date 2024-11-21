using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Transaction(decimal amount, DateTime date)
{
    [Key]
    [MaxLength(50)]
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public decimal Amount { get; set; } = amount;

    [Required] 
    public DateTime Date { get; set; } = date;

    [Required] 
    public string Status { get; set; } = "Completed";
    
    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string? UserId { get; set; }
    public virtual User? User { get; set; }

    [MaxLength(50)]
    [ForeignKey("Fundraise")]
    public string? FundraiseId { get; set; }
    public virtual Fundraise? Fundraise { get; set; }
}