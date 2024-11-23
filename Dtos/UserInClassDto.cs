using schoolMoney_backend.Models;

namespace schoolMoney_backend.Dtos;

public class UserInClassDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}