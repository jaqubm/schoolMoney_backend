namespace schoolMoney_backend.Dtos;

public class TransactionDepositDto
{
    public decimal Amount { get; set; }
    public string SourceAccountNumber { get; set; } = string.Empty;
}