using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class User(string email, string name, string surname, byte[] passwordHash, byte[] passwordSalt)
{
    [Key]
    [MaxLength(50)]
    public string UserId { get; set; } = Guid.NewGuid().ToString();

    [Required] 
    [MaxLength(255)] 
    public string Email { get; set; } = email;

    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; } = name;

    [Required] 
    [MaxLength(100)] 
    public string Surname { get; set; } = surname;
    
    [Required]
    public byte[] PasswordHash { get; set; } = passwordHash;
    
    [Required]
    public byte[] PasswordSalt { get; set; } = passwordSalt;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(50)]
    [ForeignKey("Account")]
    public string? AccountNumber { get; set; }
    public virtual Account? Account { get; set; }

    public virtual ICollection<Child>? Children { get; set; } = [];
    public virtual ICollection<Transaction>? Transactions { get; set; } = [];
    public virtual ICollection<Class>? ClassesAsTreasurer { get; set; } = [];
}