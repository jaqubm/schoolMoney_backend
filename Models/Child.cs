using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Child(string name, User parent)
{
    [Key]
    [MaxLength(50)]
    public string ChildId { get; set; } = Guid.NewGuid().ToString();

    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; } = name;

    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string ParentId { get; set; } = parent.UserId;
    public virtual User Parent { get; set; } = parent;

    [MaxLength(50)]
    [ForeignKey("Class")]
    public string? ClassId { get; set; }
    public virtual Class? Class { get; set; }
}