using schoolMoney_backend.Models;

namespace schoolMoney_backend.Dtos;

public class AccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}