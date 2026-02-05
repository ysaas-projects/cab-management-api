using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // ================================
    // ENTITY: PackagePricing
    // ================================
    public class PackagePricing
    {
        [Key]
        public int PricingId { get; set; }

        [Required]
        [ForeignKey(nameof(TourPackage))]
        public int PackageId { get; set; }

        [Required]
        [MaxLength(20)]
        public string DayType { get; set; } = null!; // Weekday / Weekend

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerPerson { get; set; }

        public int MinPersons { get; set; } = 1;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual TourPackage? TourPackage { get; set; }
    }

    // ================================
    // CREATE DTO
    // ================================
    public class CreatePackagePricingDto
    {
        [Required]
        public int PackageId { get; set; }

        [Required]
        [RegularExpression("^(Weekday|Weekend)$",
            ErrorMessage = "DayType must be Weekday or Weekend")]
        public string DayType { get; set; } = null!;

        [Required]
        public decimal PricePerPerson { get; set; }

        public int MinPersons { get; set; } = 1;
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class UpdatePackagePricingDto
    {
        [RegularExpression("^(Weekday|Weekend)$",
            ErrorMessage = "DayType must be Weekday or Weekend")]
        public string? DayType { get; set; }

        public decimal? PricePerPerson { get; set; }

        public int? MinPersons { get; set; }
    }

    // ================================
    // RESPONSE DTO
    // ================================
    public class PackagePricingResponseDto
    {
        public int PricingId { get; set; }

        public int PackageId { get; set; }
        public string? PackageName { get; set; }

        public string DayType { get; set; } = null!;

        public decimal PricePerPerson { get; set; }
        public int MinPersons { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
