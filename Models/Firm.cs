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
        public string? FirmName { get; set; }

        [Required]
        public string? FirmCode { get; set; }
        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        // ---------------- AUDIT FIELDS ----------------
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<FirmDetail> FirmDetails { get; set; } = new List<FirmDetail>();

        public ICollection<CabPrice> CabPrices { get; set; } = new List<CabPrice>();

    }
    public class FirmCreateDto
    {
        [Required]
        public string FirmName { get; set; }

        [Required]
        public string FirmCode { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class FirmDetailsFirmCreateDto // firm + firmdetail
    {
        [Required]
        public string FirmName { get; set; }

        [Required]
        public string FirmCode { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }
        public string? LogoImagePath { get; set; }
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

    public class FirmDetailsFirmUpdateDto
    {
        public int FirmId { get; set; }

        public string FirmName { get; set; }
        public string FirmCode { get; set; }
        public bool IsActive { get; set; }

        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }
        public string? LogoImagePath { get; set; }
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
