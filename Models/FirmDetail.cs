using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class FirmDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int FirmDetailsId { get; set; }  
        [ForeignKey(nameof(Firm))]
        public int FirmId { get; set; }

        public Firm Firm { get; set; }

        [Required]
        // ---------------- LOGO ----------------
        [MaxLength(1000, ErrorMessage = "Logo path cannot exceed max characters")]
        public string LogoImagePath { get; set; }
        // ---------------- ADDRESS ----------------
        [Required(ErrorMessage = "Address is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 100 characters")]
        public string Address { get; set; }

        // ---------------- CONTACT NUMBER ----------------
        [Required]
        [RegularExpression(@"^\+91[6-9][0-9]{9}$", ErrorMessage = "Enter mobile number in +91XXXXXXXXXX format")]

        public string ContactNumber { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;
        // ---------------- GST NUMBER ----------------
        [Required(ErrorMessage = "GST number is required")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Enter a valid GST number")]
        public string GstNumber { get; set; } = null!;

        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;

        // ---------------- AUDIT FIELDS ----------------
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


    }
    public class FirmDetailsResponseDto
    {
        public int FirmDetailsId { get; set; }
        public int FirmId { get; set; }
        public string FirmName { get; set; }

        public string Address { get; set; }

        // ---------------- CONTACT NUMBER ----------------

        public string ContactNumber { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;

        public string? LogoImagePath { get; set; }

        // ---------------- GST NUMBER ----------------
        public string GstNumber { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }


    public class FirmDetailsUpdateDto
    {

        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }
        public string? LogoImagePath { get; set; }
        public bool IsActive { get; set; }

        public IFormFile? Logo { get; set; }
    }
    public class FirmDetailsDto
    {
        public int FirmDetailsId { get; set; }   

        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string GstNumber { get; set; }
        public string LogoImagePath { get; set; }
        public bool IsActive { get; set; }
    }


}