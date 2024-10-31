using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Transaction(User user, decimal amount, DateTime date)
{
    [Key]
    [MaxLength(50)]
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string UserId { get; set; } = user.UserId;
    public virtual User User { get; set; } = user;

    [Required]
    public decimal Amount { get; set; } = amount;

    [Required] 
    public DateTime Date { get; set; } = date;

    [Required] 
    public string Status { get; set; } = "Completed";

    [MaxLength(50)]
    [ForeignKey("Fundraiser")]
    public string? FundraiserId { get; set; }
    public virtual Fundraiser? Fundraiser { get; set; }
}