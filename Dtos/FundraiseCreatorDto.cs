namespace schoolMoney_backend.Dtos;

public class FundraiseCreatorDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ClassId { get; set; } = string.Empty;
}