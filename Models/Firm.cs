using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class Firm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FirmId { get; set; }
        public string? FirmName { get; set; }
        public string? FirmCode { get; set; }
        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        // ---------------- AUDIT FIELDS ----------------
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    public class FirmCreateDTO
    {
        [Key]
        public string FirmName { get; set; }
        public string FirmCode { get; set; }

        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; } = true;
    }
    public class FirmUpdateDTO
    {
        [Key]
        public int FirmId { get; set; }
        public string FirmName { get; set; }
        public string FirmCode { get; set; }
        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; }

    }
    public class FirmResponseDto
    {
        public string FirmName { get; set; }
        public string FirmCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
