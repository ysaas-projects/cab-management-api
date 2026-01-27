using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
	// ================================
	// ENTITY: FirmDetail (DB Table)
	// ================================
	public class FirmDetail
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int FirmDetailsId { get; set; }

		[Required]
		[ForeignKey(nameof(Firm))]
		public int FirmId { get; set; }

		public Firm Firm { get; set; } = null!;

		// ---------------- BASIC DETAILS ----------------

		[Required]
		[StringLength(1000)]
		public string Address { get; set; } = null!;

		[Required]
		[RegularExpression(@"^\+91[6-9][0-9]{9}$")]
		public string ContactNumber { get; set; } = null!;

		public string? ContactPerson { get; set; }

		[Required]
		[RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")]
		public string GstNumber { get; set; } = null!;

		// ---------------- LOGO ----------------
		[MaxLength(1000)]
		public string? LogoImagePath { get; set; }

		// ---------------- STATUS ----------------
		public bool IsActive { get; set; } = true;
		public bool IsDeleted { get; set; } = false;

		// ---------------- AUDIT ----------------
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}

	// ================================
	// RESPONSE DTO
	// ================================
	public class FirmDetailsResponseDto
	{
		public int FirmDetailsId { get; set; }
		public int FirmId { get; set; }

		public string FirmName { get; set; } = null!;

		public string Address { get; set; } = null!;
		public string ContactNumber { get; set; } = null!;
		public string? ContactPerson { get; set; }

		public string? LogoImagePath { get; set; }

		public string GstNumber { get; set; } = null!;
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
	}

	// ================================
	// UPDATE DTO (API INPUT)
	// ================================
	public class FirmDetailsUpdateDto
	{
		public string? Address { get; set; }
		public string? ContactNumber { get; set; }
		public string? ContactPerson { get; set; }
		public string? GstNumber { get; set; }

		public bool IsActive { get; set; }

		// File upload (not mapped to DB)
		[NotMapped]
		public IFormFile? Logo { get; set; }
	}

	// ================================
	// SHARED DTO (USED IN FirmResponseDto)
	// ================================
	public class FirmDetailsDto
	{
		public int FirmDetailsId { get; set; }

		public string Address { get; set; } = null!;
		public string ContactNumber { get; set; } = null!;
		public string? ContactPerson { get; set; }

		public string GstNumber { get; set; } = null!;
		public string? LogoImagePath { get; set; }

		public bool IsActive { get; set; }
	}
}
