using DbFetcher.DTOs;

namespace DbFetcher.Services;

public interface IInvoiceService
{
    Task<(List<InvoiceDto> Data, bool HasMore, int? NextCursor, long? TotalCount)>
        GetPagedAsync(int? cursor, int pageSize);
}