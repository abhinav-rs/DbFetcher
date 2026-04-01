using DbFetcher.DTOs;

namespace DbFetcher.Services;

public interface IInvoiceService
{
    /// <summary>
    /// Get paged invoices using cursor-based pagination.
    /// Pass a null/empty Cursor on the first request; pass NextCursor from the
    /// previous response to continue to the next page.
    /// </summary>
    Task<(List<InvoiceDto> Data, bool HasMore, string? NextCursor, long? TotalCount)>
        GetPagedAsync(PagedRequestDto request);

    /// <summary>
    /// Get the total count of all invoices
    /// </summary>
    Task<long> GetTotalCountAsync();
}
