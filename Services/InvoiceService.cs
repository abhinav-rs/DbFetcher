using DbFetcher.Data;
using DbFetcher.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DbFetcher.Services;

public class InvoiceService : IInvoiceService
{
    private readonly InvoiceDbContext _context;

    public InvoiceService(InvoiceDbContext context)
    {
        _context = context;
    }

    public async Task<(List<InvoiceDto> Data, bool HasMore, int? NextCursor, long? TotalCount)>
        GetPagedAsync(int? cursor, int pageSize)
    {
        pageSize = Math.Clamp(pageSize, 1, 1000);

        var query = _context.Invoices
            .AsNoTracking()
            .OrderBy(i => i.Id);


        if (cursor.HasValue)
        {
            query = (IOrderedQueryable<Models.Invoice>)query
                .Where(i => i.Id > cursor.Value);
        }

        var rawResults = await query
            .Take(pageSize + 1)
            .Select(i => new InvoiceDto
            {
                Id                  = i.Id,
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

        var hasMore    = rawResults.Count > pageSize;
        var data       = hasMore
            ? rawResults.Take(pageSize).ToList()  
            : rawResults;

        var nextCursor = hasMore
            ? data.Last().Id
            : null as int?;

        long? totalCount = null;
        if (!cursor.HasValue)
        {
            totalCount = await GetTotalCountAsync();
        }

        return (data, hasMore, nextCursor, totalCount);
    }

    private async Task<long> GetTotalCountAsync()
    {
        var connection = _context.Database.GetDbConnection();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT SUM(rows)
            FROM sys.partitions
            WHERE object_id = OBJECT_ID('Invoices')
            AND index_id IN (0, 1)";


        await connection.OpenAsync();

        var result = await cmd.ExecuteScalarAsync();

        await connection.CloseAsync();

        return result is DBNull or null ? 0 : Convert.ToInt64(result);
    }
}