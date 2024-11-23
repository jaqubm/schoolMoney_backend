using System.ComponentModel.DataAnnotations;

namespace schoolMoney_backend.Models;

public class Account
{
    [Key]
    [MaxLength(12)]
    public string AccountNumber { get; set; } = Guid
        .NewGuid()
        .GetHashCode()
        .ToString()
        .PadLeft(12, '0')[..12];
    
    [Required]
    public decimal Balance { get; set; }

    public virtual User? User { get; set; }
    public virtual Fundraise? Fundraise { get; set; }
    public virtual List<Transaction>? SourceTransactions { get; set; }
    public virtual List<Transaction>? DestinationTransactions { get; set; }
}