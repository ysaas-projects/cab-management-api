using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class FirmTerms
    {
        [Key]
        public int FirmTermId { get; set; }

        public int FirmId { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("FirmId")]
        public Firms Firm { get; set; }

    }

    public class UpdateFirmTermDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }


    public class CreateFirmTermDto
    {
        [Required]
        public int FirmId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }




}
