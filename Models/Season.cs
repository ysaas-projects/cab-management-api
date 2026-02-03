using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // ================================
    // ENTITY: Season
    // ================================
    public class Season
    {
        [Key]
        public int SeasonId { get; set; }

        [Required]
        [ForeignKey(nameof(Firm))]
        public int FirmId { get; set; }

        [MaxLength(100)]
        public string? SeasonName { get; set; }   // Peak / Off-Season / Festival

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

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
    public class CreateSeasonDto
    {
        [MaxLength(100)]
        public string? SeasonName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class UpdateSeasonDto
    {
        [MaxLength(100)]
        public string? SeasonName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsActive { get; set; }
    }

    // ================================
    // RESPONSE DTO
    // ================================
    public class SeasonResponseDto
    {
        public int SeasonId { get; set; }

        public int FirmId { get; set; }
        public string? FirmName { get; set; }

        public string? SeasonName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
