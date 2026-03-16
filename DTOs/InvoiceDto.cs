namespace DbFetcher.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string DistributorName { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime WithdrawDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? PaymentSentOnDate { get; set; }
    public decimal InvoiceAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PoNumber { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public string ApprovedByFullName { get; set; } = string.Empty;
    public string ScheduledByFullName { get; set; } = string.Empty;
    public string InvoiceSource { get; set; } = string.Empty;
    public int PageCount { get; set; }
}