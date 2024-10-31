using schoolMoney_backend.Models;

namespace schoolMoney_backend.Dtos;

public class UserDto
{
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public Account account { get; set; }
    public ICollection<Child>? Children { get; set; }
}