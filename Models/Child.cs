using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Child
{
    [Key]
    [MaxLength(50)]
    public string ChildId { get; set; }

    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string ParentId { get; set; }
    public virtual User? Parent { get; set; }

    [MaxLength(50)]
    [ForeignKey("Class")]
    public string? ClassId { get; set; }
    public virtual Class? Class { get; set; }

    public Child()
    {
        ChildId = Guid.NewGuid().ToString();
        Name ??= string.Empty;
        ParentId ??= string.Empty;
        ClassId ??= string.Empty;
    }
}