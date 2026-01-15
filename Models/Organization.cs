using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationId { get; set; }
        public string? OrganizationName { get; set; }

        // ---------------- LOGO ----------------
        [MaxLength(1000, ErrorMessage = "Logo path cannot exceed max characters")]
        public string LogoImagePath { get; set; }

        // ---------------- ADDRESS ----------------
        [Required(ErrorMessage = "Address is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 100 characters")]
        public string Address { get; set; }

        // ---------------- MOBILE NUMBER ----------------
        [Required]
        [RegularExpression(@"^\+91[6-9][0-9]{9}$", ErrorMessage = "Enter mobile number in +91XXXXXXXXXX format")]
        public string MobileNumber { get; set; } = null!;


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
    public class OrganizationCreateDTO
    {
        [Key]
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        // ---------------- LOGO ----------------
        [MaxLength(1000, ErrorMessage = "Logo path cannot exceed max characters")]
        public string? LogoImagePath { get; set; }
        public IFormFile? LogoImage { get; set; }


        // ---------------- ADDRESS ----------------
        [Required(ErrorMessage = "Address is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 100 characters")]
        public string Address { get; set; } = null!;

        // ---------------- MOBILE NUMBER ----------------
        [Required]
        [RegularExpression(@"^\+91[6-9][0-9]{9}$", ErrorMessage = "Enter mobile number in +91XXXXXXXXXX format")]
        public string MobileNumber { get; set; } = null!;


        // ---------------- GST NUMBER ----------------
        [Required(ErrorMessage = "GST number is required")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Enter a valid GST number")]
        public string GstNumber { get; set; } = null!;



        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; } = true;

    }
    public class OrganizationUpdateDTO
    {
        [Key]
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        // ---------------- LOGO ----------------
        [MaxLength(1000, ErrorMessage = "Logo path cannot exceed max characters")]
        public string? LogoImagePath { get; set; }

        // ---------------- ADDRESS ----------------
        [Required(ErrorMessage = "Address is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 100 characters")]
        public string Address { get; set; } = null!;


        // ---------------- MOBILE NUMBER ----------------
        [Required]
        [RegularExpression(@"^\+91[6-9][0-9]{9}$", ErrorMessage = "Enter mobile number in +91XXXXXXXXXX format")]
        public string MobileNumber { get; set; } = null!;


        // ---------------- GST NUMBER ----------------
        [Required(ErrorMessage = "GST number is required")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Enter a valid GST number")]
        public string GstNumber { get; set; } = null!;



        // ---------------- STATUS FLAGS ----------------
        [Required]
        public bool IsActive { get; set; } = true;

    }

    public class OrganizationResponseDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }

 
        public string Address { get; set; }

        // ---------------- MOBILE NUMBER ----------------
        
        public string MobileNumber { get; set; } = null!;


        // ---------------- GST NUMBER ----------------
        public string GstNumber { get; set; } = null!;


        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }


}