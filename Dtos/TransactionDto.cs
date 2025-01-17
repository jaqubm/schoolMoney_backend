namespace schoolMoney_backend.Dtos;

public class TransactionDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SourceAccountNumber { get; set; } = string.Empty;
    public string DestinationAccountNumber { get; set; } = string.Empty;
}