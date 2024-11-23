using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace schoolMoney_backend.Models;

public class Class
{
    [Key]
    [MaxLength(50)]
    public string ClassId { get; set; }

    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; }

    [Required] 
    [MaxLength(100)] 
    public string SchoolName { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("User")]
    public string TreasurerId { get; set; }
    public virtual User? Treasurer { get; set; }

    public virtual ICollection<Child>? Children { get; set; } = [];
    public virtual ICollection<Fundraise>? Fundraises { get; set; } = [];

    public Class()
    {
        ClassId = Guid.NewGuid().ToString();
        Name = string.Empty;
        SchoolName = string.Empty;
        TreasurerId = string.Empty;
    }
}