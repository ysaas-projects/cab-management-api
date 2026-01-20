using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // ===================== ENTITY =====================
    [Table("CabPrices")]
    public class CabPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CabPriceId { get; set; }

        // ----------------- FOREIGN KEYS -----------------

        [Required]
        [ForeignKey(nameof(Firm))]
        public int FirmId { get; set; }

        [Required]
        [ForeignKey(nameof(Cab))]
        public int CabId { get; set; }

        [Required]
        [ForeignKey(nameof(PricingRule))]
        public int PricingRuleId { get; set; }

        // ----------------- NAVIGATION PROPERTIES -----------------

        public virtual Firm Firm { get; set; } = null!;
        public virtual Cab Cab { get; set; } = null!;
        public virtual PricingRule PricingRule { get; set; } = null!;

        // ----------------- OTHER FIELDS -----------------

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
        public int PricingRuleId { get; set; }

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

        public int? PricingRuleId { get; set; }

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
        public string FirmName { get; set; } = null!;   // ✅ NEW

        public int CabId { get; set; }
        public string CabType { get; set; } = null!;    // ✅ NEW

        public int PricingRuleId { get; set; }
        public string PricingRuleName { get; set; }   // ✅ ADD THIS


        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
