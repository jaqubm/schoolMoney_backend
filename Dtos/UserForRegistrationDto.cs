namespace schoolMoney_backend.Dtos;

public class UserForRegistrationDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}