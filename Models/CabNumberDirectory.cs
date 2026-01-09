using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    // =====================================================
    // CABNUMBERDIRECTORY ENTITY (DB TABLE)
    // =====================================================
    public class CabNumberDirectory
    {
       
        [Key]
        public int CabNumberDirectoryId { get; set; }

        [Required(ErrorMessage = "FirmId is required")]
        public int FirmId { get; set; }

        [Required(ErrorMessage = "CabId is required")]
        public int CabId { get; set; }

        [Required(ErrorMessage = "Cab Number is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Cab Number must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Z0-9\- ]+$", ErrorMessage = "Cab Number can contain only capital letters, numbers, spaces and hyphen")]
        public string CabNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    // =====================================================
    //  CREATE CABNUMBERDIRECTORY DTO 
    // =====================================================

    public class CreateCabNumberDto
    {
        [Required(ErrorMessage = "FirmId is required")]
        public int FirmId { get; set; }

        [Required(ErrorMessage = "CabId is required")]
        public int CabId { get; set; }

        [Required(ErrorMessage = "Cab Number is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Cab Number must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Z0-9\- ]+$",
            ErrorMessage = "Cab Number can contain only capital letters, numbers, spaces and hyphen")]
        public string CabNumber { get; set; }

     
        public bool IsActive { get; set; } = true;
    }

    // =====================================================
    //  UPDATE  CABNUMBERDIRECTORY  DTO 
    // =====================================================

    public class UpdateCabNumberDto
    {
        [Required(ErrorMessage = "CabNumberDirectoryId is required")]
        public int CabNumberDirectoryId { get; set; }

        [Required(ErrorMessage = "FirmId is required")]
        public int FirmId { get; set; }

        [Required(ErrorMessage = "CabId is required")]
        public int CabId { get; set; }

        [Required(ErrorMessage = "Cab Number is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Cab Number must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Z0-9\- ]+$",
            ErrorMessage = "Cab Number can contain only capital letters, numbers, spaces and hyphen")]
        public string CabNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    }
}
