namespace DbFetcher.DTOs;

public class PagedResponseDto<T>
{
    public List<T> Data { get; set; } = [];

    public MetadataDto Metadata { get; set; } = new();
}

public class MetadataDto
{
    public int? NextCursor { get; set; }

    public bool HasMore { get; set; }

    public int PageSize { get; set; }


    public long? TotalCount { get; set; }
}


