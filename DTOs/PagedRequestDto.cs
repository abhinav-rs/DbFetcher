namespace DbFetcher.DTOs;

public class PagedRequestDto
{

    public int? Cursor { get; set; }

    public int PageSize { get; set; } = 50;
}