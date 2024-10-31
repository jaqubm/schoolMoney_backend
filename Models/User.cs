using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class User(string email, string passwordHash, Account account)
{
    [Key]
    [MaxLength(50)]
    public string UserId { get; set; } = Guid.NewGuid().ToString();

    [Required] 
    [MaxLength(255)] 
    public string Email { get; set; } = email;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = passwordHash;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(50)]
    [ForeignKey("Account")]
    public string AccountNumber { get; set; } = account.AccountNumber;
    public virtual Account Account { get; set; } = account;

    public virtual ICollection<Child>? Children { get; set; } = [];
    public virtual ICollection<Transaction>? Transactions { get; set; } = [];
    public virtual ICollection<Class>? ClassesAsTreasurer { get; set; } = [];
}