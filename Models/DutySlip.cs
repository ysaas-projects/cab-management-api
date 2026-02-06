using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DriverDetails.Models;

namespace cab_management.Models
{
    // ===================== ENTITY =====================
    public class DutySlip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DutySlipId { get; set; }

        [Required]
        public DateTime BookedDate { get; set; }

        [Required]
        public int BookedBy { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        //[Required]
        public DateTime? ReportingDateTime { get; set; }

        [StringLength(500)]
        public string? ReportingAddress { get; set; }

        [StringLength(255)]
        public string? ReportingGeoLocation { get; set; }

        public int? RequestedCab { get; set; }
        public int? SentCab { get; set; }

        [StringLength(50)]
        public string? CabNumber { get; set; }

        //[Required]
        public int? DriverDetailId { get; set; }

        [StringLength(50)]
        public string? PaymentMode { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? StartKms { get; set; }

        [StringLength(255)]
        public string? StartKmsImagePath { get; set; }

        public DateTime? StartDateTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CloseKms { get; set; }

        [StringLength(255)]
        public string? CloseKmsImagePath { get; set; }

        public DateTime? CloseDateTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalKms { get; set; }

        public int? TotalTimeInMin { get; set; }

        [StringLength(500)]
        public string? NextDayInstruction { get; set; }

        [StringLength(255)]
        public string? Destination { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsBillingLocked { get; set; } = false;

		[ForeignKey(nameof(FirmId))]
        public virtual Firm Firm { get; set; }


        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }        

        [ForeignKey(nameof(DriverDetailId))]
        public virtual DriverDetail DriverDetail { get; set; }   // ✅ ADD

        [ForeignKey(nameof(RequestedCab))]
        public virtual Cab RequestedCabNav { get; set; }         // ✅ ADD

        [ForeignKey(nameof(SentCab))]
        public virtual Cab SentCabNav { get; set; }               // ✅ ADD


    }


	public class DutyExpenseDto
	{
		public int DutyExpenseId { get; set; }
		public int DutyId { get; set; }
		public string ExpenseType { get; set; }
		public string? Description { get; set; }
		public string ExpenseAmount { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class DutySlipWithExpensesDto
	{
		public DutySlipResponseDto DutySlip { get; set; }
		public List<DutyExpenseDto> Expenses { get; set; } = new();
		public decimal TotalExpenseAmount { get; set; }
	}


	// ===================== CREATE DTO =====================
	public class CreateDutySlipDto
    {
        [Required]
        public DateTime BookedDate { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? RequestedCab { get; set; }

        public string? Destination { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Select at least one customer user")]
        public List<int> CustomerUserIds { get; set; } = new();


        // ✅ DEFAULT STATUS
        public string Status { get; set; } = "Booked";
    }
   
    // ===================== ASSIGN DRIVER UPDATE DTO =====================
    public class UpdateAssignDriverDto
    {
        //[Required]
        public int? DriverDetailId { get; set; }

        [Required]
        [StringLength(500)]
        public string ReportingAddress { get; set; }

        //[Required]
        public DateTime? ReportingDateTime { get; set; }

        public int? SentCab { get; set; }
        public string? CabNumber { get; set; }

        // ✅ STATUS
        public string Status { get; set; } = "De-Assigned";
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateStartJourneyDto
    {
        [StringLength(255)]
        public string? ReportingGeoLocation { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? StartKms { get; set; }

        [StringLength(255)]
        public string? StartKmsImagePath { get; set; }

        public DateTime? StartDateTime { get; set; }

        // ✅ STATUS
        public string Status { get; set; } = "Start-Journey";
        public DateTime? UpdatedAt { get; set; }
    }

    // ===================== END JOURNEY UPDATE DTO =====================
    public class UpdateEndJourneyDto
    {
        [Column(TypeName = "decimal(10,2)")]
        public decimal? CloseKms { get; set; }

        [StringLength(255)]
        public string? CloseKmsImagePath { get; set; }

        public DateTime? CloseDateTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalKms { get; set; }

        public int? TotalTimeInMin { get; set; }

        // ✅ STATUS
        public string Status { get; set; } = "End-Journey";
        public DateTime? UpdatedAt { get; set; }
    }
    
    // ===================== INSTRUCTION UPDATE DTO =====================
    public class UpdateInstructionDto
    {
        [Required]
        [StringLength(500)]
        public string NextDayInstruction { get; set; }

        public string Status { get; set; } = "Instructed";

        public DateTime? UpdatedAt { get; set; }
    }

    // ===================== BILLING UPDATE DTO =====================
    public class UpdateBillingDto
    {
        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; }

        // ✅ STATUS
        public string Status { get; set; } = "Bill-Pending";

        public DateTime? UpdatedAt { get; set; }
    }


    public class DutySlipResponseDto
    {
        public int DutySlipId { get; set; }

        public DateTime? BookedDate { get; set; }
        public int? BookedBy { get; set; }

        public int FirmId { get; set; }
        public string? FirmName { get; set; }

        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }

        // ✅ NEW (FOR INVOICE - TO SECTION)
        public string? CustomerAddress { get; set; }
        public string? CustomerContactNumber { get; set; }
        public string? CustomerGstNumber { get; set; }

        public int? DriverDetailId { get; set; }
        public string? DriverName { get; set; }

        public int? RequestedCab { get; set; }
        public string? RequestedCabType { get; set; }

        public int? SentCab { get; set; }
        public string? SentCabType { get; set; }

        public string? CabNumber { get; set; }

        public decimal? StartKms { get; set; }
        public DateTime? StartDateTime { get; set; }

        public decimal? CloseKms { get; set; }
        public DateTime? CloseDateTime { get; set; }

        public decimal? TotalKms { get; set; }
        public int? TotalTimeInMin { get; set; }

        public string? Destination { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }





}
