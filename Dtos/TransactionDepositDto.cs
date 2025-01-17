namespace schoolMoney_backend.Dtos;

public class TransactionDepositDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string SourceAccountNumber { get; set; } = string.Empty;
}