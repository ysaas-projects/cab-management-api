using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // =====================================================
    // DUTY LOCATIONS ENTITY (DB TABLE)
    // =====================================================
    public class DutyLocations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DutyLocationId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GeoLocation { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    // =====================================================
    // CREATE DUTY LOCATION DTO
    // =====================================================
    public class CreateDutyLocationDTO
    {
        [Required]
        public int FirmId { get; set; }

        [Required]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GeoLocation { get; set; }
    }

    // =====================================================
    // UPDATE DUTY LOCATION DTO
    // =====================================================
    public class UpdateDutyLocationDTO
    {
        [Required]
        public int FirmId { get; set; }

        [Required]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GeoLocation { get; set; }

        public bool IsActive { get; set; }
    }
}
