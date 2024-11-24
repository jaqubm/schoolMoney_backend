namespace schoolMoney_backend.Dtos;

public class FundraiseListDto
{
    public string FundraiseId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public bool IsTreasurer { get; set; }
}