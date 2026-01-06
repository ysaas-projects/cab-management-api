using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class Firms
    {
        [Key]
        public int FirmId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirmName { get; set; }

        [StringLength(20)]
        public string FirmCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // ✅ Navigation Property (One Firm → Many FirmTerms)
        public ICollection<FirmTerms> FirmTerms { get; set; }
    }
}
