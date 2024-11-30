namespace schoolMoney_backend.Dtos;

public class FundraiseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ImageIndex { get; set; }
    public decimal GoalAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public int TotalSupporters { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public bool IsTreasurer { get; set; }
}