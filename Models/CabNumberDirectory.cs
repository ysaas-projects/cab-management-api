using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class CabNumberDirectory
    {

        [Key]
        public int CabNumberDirectoryId { get; set; }

        [ForeignKey(nameof(Firm))]
        public int FirmId { get; set; }

        [ForeignKey(nameof(Cab))]

        public int CabId { get; set; }

        [Required(ErrorMessage = "Cab Number is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Cab Number must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Z0-9\- ]+$", ErrorMessage = "Cab Number can contain only capital letters, numbers, spaces and hyphen")]
        public string CabNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public Firm Firm { get; set; } = null!;
        public Cab Cab { get; set; } = null!;
    }


    public class CreateCabNumberDto
    {

        public int CabId { get; set; }

        [Required(ErrorMessage = "Cab Number is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Cab Number must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Z0-9\- ]+$",
            ErrorMessage = "Cab Number can contain only capital letters, numbers, spaces and hyphen")]
        public string CabNumber { get; set; }


        public bool IsActive { get; set; } = true;
    }


    public class UpdateCabNumberDto
    {
        [Required(ErrorMessage = "CabNumberDirectoryId is required")]
        public int CabNumberDirectoryId { get; set; }


        [Required(ErrorMessage = "CabId is required")]
        public int CabId { get; set; }

        [Required(ErrorMessage = "Cab Number is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Cab Number must be between 3 and 20 characters")]
        [RegularExpression(@"^[A-Z0-9\- ]+$",
            ErrorMessage = "Cab Number can contain only capital letters, numbers, spaces and hyphen")]
        public string CabNumber { get; set; }

        public bool IsActive { get; set; }


    }

    public class CabNumberDirectoryResponseDto
    {
        public int CabNumberDirectoryId { get; set; }

        public int FirmId { get; set; }
        public string? FirmName { get; set; }

        public int CabId { get; set; }
        public string? CabType { get; set; }

        public string CabNumber { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // ===============================
    // CAB WISE RESPONSE DTO (JOIN)
    // ===============================
    public class CabWithNumbersDto
    {
        public int CabId { get; set; }
        public string CabType { get; set; } = null!;
        public List<CabNumberOnlyDto> CabNumbers { get; set; } = new();
    }

    public class CabNumberOnlyDto
    {
        public int CabNumberDirectoryId { get; set; }
        public string CabNumber { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}