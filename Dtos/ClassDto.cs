using schoolMoney_backend.Models;

namespace schoolMoney_backend.Dtos;

public class ClassDto
{
    public string Name { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public UserInClassDto Treasurer { get; set; } = new();
    public IEnumerable<ChildInClassDto> Children { get; set; } = [];
    public IEnumerable<Fundraise> Fundraise { get; set; } = []; // TODO: Add DTO when created
}