namespace DbFetcher.DTOs;

public class PagedResponseDto<T>
{
    public List<T> Data { get; set; } = [];

    public MetadataDto Metadata { get; set; } = new();
}

public class MetadataDto
{
    /// <summary>
    /// Next cursor for cursor-based pagination or index indicator for index-based pagination
    /// </summary>
    public string? NextCursor { get; set; }

    public bool HasMore { get; set; }

    public int PageSize { get; set; }

    public long? TotalCount { get; set; }
}


