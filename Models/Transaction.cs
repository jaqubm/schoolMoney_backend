using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Transaction
{
    [Key]
    [MaxLength(50)]
    public string TransactionId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required] 
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; }
    
    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual User? User { get; set; }

    [MaxLength(50)]
    [ForeignKey("Fundraise")]
    public string? FundraiseId { get; set; }
    public virtual Fundraise? Fundraise { get; set; }

    public Transaction()
    { 
        TransactionId = Guid.NewGuid().ToString();
        Date = DateTime.Now;
        Type ??= string.Empty;
        Status = "Completed";
        UserId ??= string.Empty;
    }
}