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
    [MaxLength(12)]
    [ForeignKey("Account")]
    public string SourceAccountNumber { get; set; }
    public virtual Account? SourceAccount { get; set; }

    [Required]
    [MaxLength(12)]
    [ForeignKey("Account")]
    public string DestinationAccountNumber { get; set; }
    public virtual Account? DestinationAccount { get; set; }

    public Transaction()
    { 
        TransactionId = Guid.NewGuid().ToString();
        Date = DateTime.Now;
        Status = "Completed";   // <- Status only for simulation
        Type ??= string.Empty;
        SourceAccountNumber ??= string.Empty;
        DestinationAccountNumber ??= string.Empty;
    }
}