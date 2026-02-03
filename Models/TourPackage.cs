using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // ================================
    // ENTITY: TourPackage
    // ================================
    public class TourPackage
    {
        [Key]
        public int PackageId { get; set; }

        [Required]
        [ForeignKey(nameof(Firm))]
        public int FirmId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public int? DurationDays { get; set; }

        public int? DurationNights { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? BasePrice { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Firm? Firm { get; set; }
    }

    // ================================
    // CREATE DTO
    // ================================
    public class CreateTourPackageDto
    {
        [Required]
        [MaxLength(200)]
        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public int? DurationDays { get; set; }

        public int? DurationNights { get; set; }

        public decimal? BasePrice { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class UpdateTourPackageDto
    {
        [MaxLength(200)]
        public string? PackageName { get; set; }

        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public int? DurationDays { get; set; }

        public int? DurationNights { get; set; }

        public decimal? BasePrice { get; set; }

        public bool? IsActive { get; set; }
    }

    // ================================
    // RESPONSE DTO
    // ================================
    public class TourPackageResponseDto
    {
        public int PackageId { get; set; }

        public int FirmId { get; set; }
        public string? FirmName { get; set; }

        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }
        public string? Location { get; set; }

        public int? DurationDays { get; set; }
        public int? DurationNights { get; set; }

        public decimal? BasePrice { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
