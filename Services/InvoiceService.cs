using DbFetcher.Data;
using DbFetcher.DTOs;
using DbFetcher.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DbFetcher.Services;

public class InvoiceService : IInvoiceService
{
    private readonly InvoiceDbContext _context;

    public InvoiceService(InvoiceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get paged invoices using cursor-based pagination.
    /// Omit Cursor to start from the beginning; supply NextCursor from the
    /// previous response to fetch the next page.
    /// </summary>
    public async Task<(List<InvoiceDto> Data, bool HasMore, string? NextCursor, long? TotalCount)>
        GetPagedAsync(PagedRequestDto request)
    {
        request.PageSize = Math.Clamp(request.PageSize < 1 ? 50 : request.PageSize, 1, 1000);

        var collection    = _context.Invoices;
        var filterBuilder = Builders<Invoice>.Filter;
        FilterDefinition<Invoice> filter = filterBuilder.Empty;

        // Apply cursor filter if a cursor was provided
        if (!string.IsNullOrEmpty(request.Cursor))
        {
            if (ObjectId.TryParse(request.Cursor, out var cursorObjectId))
            {
                filter = filterBuilder.Gt(i => i.Id, cursorObjectId);
            }
            // If the cursor string is not a valid ObjectId ignore it silently
            // and return from the beginning (safe fallback)
        }

        // Fetch pageSize + 1 so we can determine whether there is a next page
        var rawResults = await collection
            .Find(filter)
            .Sort(Builders<Invoice>.Sort.Ascending(i => i.Id))
            .Limit(request.PageSize + 1)
            .Project(i => new InvoiceDto
            {
                Id                  = i.Id.ToString(),
                InvoiceNumber       = i.InvoiceNumber,
                DistributorName     = i.DistributorName,
                LocationId          = i.LocationId,
                LocationName        = i.LocationName,
                InvoiceDate         = i.InvoiceDate,
                ReceivedDate        = i.ReceivedDate,
                DueDate             = i.DueDate,
                ApprovedDate        = i.ApprovedDate,
                ScheduledDate       = i.ScheduledDate,
                WithdrawDate        = i.WithdrawDate,
                PaymentDate         = i.PaymentDate,
                PaymentSentOnDate   = i.PaymentSentOnDate,
                InvoiceAmount       = i.InvoiceAmount,
                PaymentAmount       = i.PaymentAmount,
                Status              = i.Status,
                PaymentType         = i.PaymentType,
                PaymentMethod       = i.PaymentMethod,
                PoNumber            = i.PoNumber,
                ReferenceNumber     = i.ReferenceNumber,
                ApprovedBy          = i.ApprovedBy,
                ApprovedByFullName  = i.ApprovedByFullName,
                ScheduledByFullName = i.ScheduledByFullName,
                InvoiceSource       = i.InvoiceSource,
                PageCount           = i.PageCount
            })
            .ToListAsync();

        var hasMore    = rawResults.Count > request.PageSize;
        var data       = hasMore ? rawResults.Take(request.PageSize).ToList() : rawResults;
        var nextCursor = hasMore ? data.Last().Id : null;

        // Single count call — only done once per request
        long? totalCount = await GetTotalCountAsync();

        return (data, hasMore, nextCursor, totalCount);
    }

    public async Task<long> GetTotalCountAsync()
    {
        return await _context.Invoices
            .CountDocumentsAsync(Builders<Invoice>.Filter.Empty);
    }
}