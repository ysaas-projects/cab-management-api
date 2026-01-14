using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class Firm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FirmId { get; set; }

        [Required]
        public string FirmName { get; set; } = null!;

        [Required]
        public string FirmCode { get; set; } = null!;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // One-to-One
        public FirmDetail FirmDetails { get; set; } = null!;
    }
    public class FirmCreateDto
    {
        [Required]
        public string FirmName { get; set; }

        [Required]
        public string FirmCode { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class FirmUpdateDto
    {
        [Required]
        public int FirmId { get; set; }

        [Required]
        public string FirmName { get; set; }

        [Required]
        public string FirmCode { get; set; }

        public bool IsActive { get; set; }
    }


    public class FirmResponseDto
    {
        public int FirmId { get; set; }
        public string FirmName { get; set; }
        public string FirmCode { get; set; }
        public bool IsActive { get; set; }

        public FirmDetailsDto? FirmDetails { get; set; }
    }

}
