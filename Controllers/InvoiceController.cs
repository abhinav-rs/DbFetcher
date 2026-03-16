using DbFetcher.DTOs;
using DbFetcher.Services;
using Microsoft.AspNetCore.Mvc;

namespace DbFetcher.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices([FromQuery] PagedRequestDto request)
    {
        if (request.PageSize < 1)
        {
            return BadRequest(new { message = "PageSize must be greater than 0." });
        }

        var (data, hasMore, nextCursor, totalCount) = await _invoiceService
            .GetPagedAsync(request.Cursor, request.PageSize);

        var response = new PagedResponseDto<InvoiceDto>
        {
            Data = data,
            Metadata = new MetadataDto
            {
                NextCursor = nextCursor,
                HasMore    = hasMore,
                PageSize   = data.Count,
                TotalCount = totalCount
            }
        };

        return Ok(response);
    }
}
