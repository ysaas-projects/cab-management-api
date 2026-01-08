using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // ===================== ENTITY =====================
    public class CabPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CabPriceId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int CabId { get; set; }

        [Required]
        public int PriceRuleId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    // ===================== CREATE DTO =====================
    public class CreateCabPriceDto
    {
        [Required]
        public int FirmId { get; set; }

        [Required]
        public int CabId { get; set; }

        [Required]
        public int PriceRuleId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
    }

    // ===================== UPDATE DTO =====================
    public class UpdateCabPriceDto
    {
        [Required]
        public int FirmId { get; set; }

        public int? CabId { get; set; }

        public int? PriceRuleId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    // ===================== RESPONSE DTO =====================
    public class CabPriceResponseDto
    {
        public int CabPriceId { get; set; }
        public int FirmId { get; set; }
        public int CabId { get; set; }
        public int PriceRuleId { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
