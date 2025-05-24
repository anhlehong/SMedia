namespace Application.DTOs;

public class AdvancedSearchDto
{
    public string? Query { get; set; }
    public string? Gender { get; set; }
    public DateTime? JoinedAfter { get; set; }
    public string? Status { get; set; }
}