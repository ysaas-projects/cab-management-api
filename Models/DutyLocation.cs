namespace cab_management.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    // =====================================================
    // DUTY LOCATIONS(Db Set)
    // =====================================================
    public partial class DutyLocation
    {
        [Key]
        public int DutyLocationId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int DutyId { get; set; }

        [Required]
         public string Address { get; set; } = string.Empty;

         public string? GeoLocation { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    // =====================================================
    // CREATE DUTYLOCATION DTO
    // =====================================================
    public class AddDutyLocationDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "FirmId must be greater than 0")]
        public int FirmId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DutyId must be greater than 0")]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Duty address is required")]
        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(150)]
        public string? GeoLocation { get; set; }

        public bool IsActive { get; set; } = true;
    }


    // =====================================================
    // UPDATE DUTYLOCATION DTO
    // =====================================================

    public class UpdateDutyLocationDTO
    {
        [Required]
        public int DutyLocationId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int FirmId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Duty address is required")]
        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(150)]
        public string? GeoLocation { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
