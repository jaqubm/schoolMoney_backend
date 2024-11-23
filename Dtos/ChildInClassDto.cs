namespace schoolMoney_backend.Dtos;

public class ChildInClassDto
{
    public string ChildId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentSurname { get; set; } = string.Empty;
}