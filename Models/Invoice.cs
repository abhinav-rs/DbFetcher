using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DbFetcher.Models;

public class Invoice
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("invoiceNumber")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [BsonElement("distributorName")]
    public string DistributorName { get; set; } = string.Empty;

    [BsonElement("locationId")]
    public string LocationId { get; set; } = string.Empty;

    [BsonElement("locationName")]
    public string LocationName { get; set; } = string.Empty;

    [BsonElement("invoiceDate")]
    public DateTime? InvoiceDate { get; set; }

    [BsonElement("receivedDate")]
    public DateTime? ReceivedDate { get; set; }

    [BsonElement("dueDate")]
    public DateTime? DueDate { get; set; }

    [BsonElement("approvedDate")]
    public DateTime? ApprovedDate { get; set; }

    [BsonElement("scheduledDate")]
    public DateTime? ScheduledDate { get; set; }

    [BsonElement("withdrawDate")]
    public DateTime? WithdrawDate { get; set; }

    [BsonElement("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [BsonElement("paymentSentOnDate")]
    public DateTime? PaymentSentOnDate { get; set; }

    [BsonElement("invoiceAmount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal InvoiceAmount { get; set; }

    [BsonElement("paymentAmount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal PaymentAmount { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("paymentType")]
    public string PaymentType { get; set; } = string.Empty;

    [BsonElement("paymentMethod")]
    public string PaymentMethod { get; set; } = string.Empty;

    [BsonElement("poNumber")]
    public string PoNumber { get; set; } = string.Empty;

    [BsonElement("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    [BsonElement("approvedBy")]
    public string ApprovedBy { get; set; } = string.Empty;

    [BsonElement("approvedByFullName")]
    public string ApprovedByFullName { get; set; } = string.Empty;

    [BsonElement("scheduledByFullName")]
    public string ScheduledByFullName { get; set; } = string.Empty;

    [BsonElement("invoiceSource")]
    public string InvoiceSource { get; set; } = string.Empty;

    [BsonElement("pageCount")]
    public int PageCount { get; set; }
}