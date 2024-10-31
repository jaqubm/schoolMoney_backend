using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Class(string name, string schoolName, User treasurer)
{
    [Key]
    [MaxLength(50)]
    public string ClassId { get; set; } = Guid.NewGuid().ToString();

    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; } = name;

    [Required] 
    [MaxLength(100)] 
    public string SchoolName { get; set; } = schoolName;

    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string TreasurerId { get; set; } = treasurer.UserId;
    public virtual User Treasurer { get; set; } = treasurer;

    public virtual ICollection<Child>? Children { get; set; } = [];
    public virtual ICollection<Fundraiser>? Fundraisers { get; set; } = [];
}