using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class User
{
    [Key]
    [MaxLength(50)]
    public string UserId { get; set; }

    [Required] 
    [MaxLength(255)] 
    public string Email { get; set; }

    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; }

    [Required] 
    [MaxLength(100)] 
    public string Surname { get; set; }
    
    [Required]
    public byte[] PasswordHash { get; set; }
    
    [Required]
    public byte[] PasswordSalt { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("Account")]
    public string? AccountNumber { get; set; }
    public virtual Account? Account { get; set; }

    public virtual ICollection<Child>? Children { get; set; } = [];
    public virtual ICollection<Transaction>? Transactions { get; set; } = [];
    public virtual ICollection<Class>? ClassesAsTreasurer { get; set; } = [];

    public User()
    {
        UserId = Guid.NewGuid().ToString();
        Email ??= string.Empty;
        Name ??= string.Empty;
        Surname ??= string.Empty;
        PasswordHash ??= [];
        PasswordSalt ??= [];
        CreatedAt = DateTime.Now;
    }
}