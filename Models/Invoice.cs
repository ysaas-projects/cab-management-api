using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [StringLength(50)]
        public string? IterneryCode { get; set; }

        [Required]
        public int DutySlipId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    // ===================== CREATE DTO =====================
    public class CreateInvoiceDto
    {
        [Required]
        public int FirmId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public int DutySlipId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string? IterneryCode { get; set; }
    }

    // ===================== UPDATE DTO =====================
    public class UpdateInvoiceDto
    {
        [Column(TypeName = "decimal(12,2)")]
        public decimal? TotalAmount { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    // ===================== RESPONSE DTO =====================
    public class InvoiceResponseDto
    {
        public int InvoiceId { get; set; }
        public int FirmId { get; set; }
        public int CustomerId { get; set; }
        public int DutySlipId { get; set; }

        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string? IterneryCode { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}