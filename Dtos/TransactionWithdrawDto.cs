namespace schoolMoney_backend.Dtos;

public class TransactionWithdrawDto
{
    public decimal Amount { get; set; }
    public string DestinationAccountNumber { get; set; } = string.Empty;
}