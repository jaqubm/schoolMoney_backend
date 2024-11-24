using schoolMoney_backend.Models;

namespace schoolMoney_backend.Dtos;

public class ClassDto
{
    public string Name { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public bool IsTreasurer  { get; set; }
    public UserInClassDto Treasurer { get; set; } = new();
    public IEnumerable<ChildInClassDto> Children { get; set; } = [];
}