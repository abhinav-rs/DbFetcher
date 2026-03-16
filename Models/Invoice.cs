using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbFetcher.Models;

[Table("Invoices")]
public class Invoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DistributorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LocationId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string LocationName { get; set; } = string.Empty;

    [Required]
    public DateTime InvoiceDate { get; set; }

    [Required]
    public DateTime ReceivedDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public DateTime ApprovedDate { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [Required]
    public DateTime WithdrawDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? PaymentSentOnDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal InvoiceAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PaymentAmount { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string PaymentType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string PoNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ApprovedByFullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ScheduledByFullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string InvoiceSource { get; set; } = string.Empty;

    [Required]
    public int PageCount { get; set; }
}