using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
	// ================================
	// ENTITY: Firm
	// ================================
	public class Firm
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int FirmId { get; set; }

		[Required]
		public string FirmName { get; set; } = null!;

		[Required]
		public string FirmCode { get; set; } = null!;

		// ---------------- STATUS FLAGS ----------------
		public bool IsActive { get; set; } = true;
		public bool IsDeleted { get; set; } = false;

		// ---------------- AUDIT FIELDS ----------------
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		// ---------------- NAVIGATION ----------------
		// One Firm → Many FirmDetails (you usually pick the active one)
		public ICollection<FirmDetail> FirmDetails { get; set; } = new List<FirmDetail>();

		// One Firm → Many CabPrices
		public ICollection<CabPrice> CabPrices { get; set; } = new List<CabPrice>();
	}

	// ================================
	// CREATE DTO
	// ================================
	public class FirmCreateDto
	{
		[Required]
		public string FirmName { get; set; } = null!;

		[Required]
		public string FirmCode { get; set; } = null!;

		public bool IsActive { get; set; } = true;
	}

    // ================================
    // CREATE (Firm + FirmDetail)
    // ================================
    public class FirmDetailsFirmCreateDto
    {
        [Required]
        public string FirmName { get; set; } = null!;

        [Required]
        public string FirmCode { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? GstNumber { get; set; }

        public IFormFile? Logo { get; set; }   // ✅ ADD THIS
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class FirmUpdateDto
	{
		[Required]
		public int FirmId { get; set; }

		[Required]
		public string FirmName { get; set; } = null!;

		[Required]
		public string FirmCode { get; set; } = null!;

		public bool IsActive { get; set; }
	}

	// ================================
	// UPDATE (Firm + FirmDetail)
	// ================================
	public class FirmDetailsFirmUpdateDto
	{
		public int FirmId { get; set; }

		public string FirmName { get; set; } = null!;
		public string FirmCode { get; set; } = null!;
		public bool IsActive { get; set; }

		public string? Address { get; set; }
		public string? ContactNumber { get; set; }
		public string? ContactPerson { get; set; }
		public string? GstNumber { get; set; }
		public string? LogoImagePath { get; set; }
	}

	// ================================
	// RESPONSE DTO
	// ================================
	public class FirmResponseDto
	{
		public int FirmId { get; set; }

		public string FirmName { get; set; } = null!;
		public string FirmCode { get; set; } = null!;
		public bool IsActive { get; set; }

		public FirmDetailsDto? FirmDetails { get; set; }
	}
}
