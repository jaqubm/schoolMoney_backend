namespace schoolMoney_backend.Dtos;

public class TransactionTransferDto
{
    public decimal Amount { get; set; }
    public string DestinationAccountNumber { get; set; } = string.Empty;
}