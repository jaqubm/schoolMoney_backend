namespace schoolMoney_backend.Dtos;

public class TransactionTransferDto
{
    public decimal Amount { get; set; }
    public string SourceAccountNumber { get; set; } = string.Empty;
    public string DestinationAccountNumber { get; set; } = string.Empty;
}