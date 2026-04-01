namespace DbFetcher.DTOs;

public class PagedRequestDto
{
    /// <summary>
    /// Cursor for cursor-based pagination (ObjectId string of the last seen record)
    /// Omit on the first request; pass NextCursor from the previous response to get the next page.
    /// </summary>
    public string? Cursor { get; set; }

    /// <summary>
    /// Number of records per page (default: 50, max: 1000)
    /// </summary>
    public int PageSize { get; set; } = 50;
}