using System.ComponentModel.DataAnnotations;

namespace schoolMoney_backend.Models;

public class Account
{
    [Key]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = Guid
        .NewGuid()
        .GetHashCode()
        .ToString()
        .PadLeft(12, '0')[..12];
    
    [Required]
    public decimal Balance { get; set; }

    public virtual ICollection<User>? Users { get; set; } = [];
    public virtual ICollection<Fundraise>? Fundraises { get; set; } = [];
}