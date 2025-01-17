namespace schoolMoney_backend.Dtos;

public class TransactionWithdrawDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string DestinationAccountNumber { get; set; } = string.Empty;
}