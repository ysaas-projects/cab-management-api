using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{

    public class Cab
    {
        [Key]
        public int CabId { get; set; }

        [ForeignKey(nameof(Firm))]

        public int FirmId { get; set; }
        [Required]
        [MaxLength(50)]
        public string CabType { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual Firm? Firm { get; set; }
    }
    public class CreateCabDto
    {
        [Required]
        [MaxLength(50)]
        public string CabType { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCabDto
    {
        [MaxLength(50)]
        public string? CabType { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CabResponseDto
    {
        public int CabId { get; set; }
        public int FirmId { get; set; }
        public string? FirmName { get; set; }
        public string CabType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
