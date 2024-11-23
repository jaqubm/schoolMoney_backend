using schoolMoney_backend.Models;

namespace schoolMoney_backend.Dtos;

public class UserDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public AccountDto Account { get; set; } = new();
    public IEnumerable<ChildDto>? Children { get; set; } = [];
}