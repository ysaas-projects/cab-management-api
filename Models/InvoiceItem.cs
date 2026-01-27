using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
       public class InvoiceItem
       {
        // ===================== ENTITY MODEL =====================
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceItemId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

       }
        // ===================== CREATE DTO =====================
        public class CreateInvoiceItemDto
        {
            [Required]
            public int FirmId { get; set; }

            [Required]
            public int InvoiceId { get; set; }

            [Required]
            [StringLength(255)]
            public string Particulars { get; set; }

            [Required]
            public int Quantity { get; set; }

            [Required]
            public decimal Price { get; set; }

            [Required]
            public decimal TotalPrice { get; set; }

            public bool IsActive { get; set; } = true;

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        // ===================== UPDATE DTO =====================
        public class UpdateInvoiceItemDto
        {
            public int? FirmId { get; set; }

            public int? InvoiceId { get; set; }

            [StringLength(255)]
            public string? Particulars { get; set; }

            public int? Quantity { get; set; }

            public decimal? Price { get; set; }

            public decimal? TotalPrice { get; set; }

            public bool? IsActive { get; set; }

            public DateTime? UpdatedAt { get; set; }
        }

        // ===================== RESPONSE DTO =====================
        public class InvoiceItemResponseDto
        {
            public int InvoiceItemId { get; set; }
            public int FirmId { get; set; }
            public int InvoiceId { get; set; }
            public string Particulars { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsDeleted { get; set; }
        }
    
}
