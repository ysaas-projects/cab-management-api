using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class PricingRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PricingRuleId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        [StringLength(500)]
        public string RuleDetails { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        


    }

    // ===================== CREATE DTO =====================
    public class CreatePricingRuleDto
    {
       
        [Required]
        public int FirmId { get; set; }
        [Required]
        [StringLength(500)]
        public string RuleDetails { get; set; }
       
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;


    }

    // ===================== Update DTO =====================
    public class UpdatePricingRuleDto
    {
        
        [Required]
        public int FirmId { get; set; }



        [StringLength(500)]
        public string? RuleDetails { get; set; }
        public bool? IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    // ===================== RESPONSE DTO =====================
    public class PricingRuleResponseDto
    {
        public int PricingRuleId { get; set; }
        public int FirmId { get; set; }
        public string RuleDetails { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }



}
