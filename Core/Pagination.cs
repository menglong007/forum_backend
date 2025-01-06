namespace WebApplication1.Core;

public class AppPaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Filter { get; set; }
    public string? SortField { get; set; }
    public bool? Descending { get; set; }
}