using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public int? DriverId { get; set; }

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
    }

    // ===================== CREATE DTO =====================
    public class CreateDutySlipDto
    {
        [Required]
        public DateTime BookedDate { get; set; }

        [Required]
        public int BookedBy { get; set; }

        [Required]
        public int FirmId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? RequestedCab { get; set; }

        public string? Destination { get; set; }

        // ✅ DEFAULT STATUS
        public string Status { get; set; } = "Booked";
    }
   
    // ===================== ASSIGN DRIVER UPDATE DTO =====================
    public class UpdateAssignDriverDto
    {
        //[Required]
        public int? DriverId { get; set; }

        [Required]
        [StringLength(500)]
        public string ReportingAddress { get; set; }

        //[Required]
        public DateTime? ReportingDateTime { get; set; }

        public int? SentCab { get; set; }
        public string? CabNumber { get; set; }

        // ✅ STATUS
        public string Status { get; set; } = "Driver-Assigned";
        public DateTime? UpdatedAt { get; set; }
    }

    // ===================== START JOURNEY UPDATE DTO =====================
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
        public int? DriverId { get; set; }
        public string? ReportingAddress { get; set; }
        public DateTime? ReportingDateTime { get; set; }
        public int? SentCab { get; set; }
        public string? CabNumber { get; set; }

        public string? ReportingGeoLocation { get; set; }

        public decimal? StartKms { get; set; }

        public string? StartKmsImagePath { get; set; }

        public DateTime? StartDateTime { get; set; }
        public decimal? CloseKms { get; set; }

        public string? CloseKmsImagePath { get; set; }

        public DateTime? CloseDateTime { get; set; }

        public decimal? TotalKms { get; set; }

        public int? TotalTimeInMin { get; set; }
        public string NextDayInstruction { get; set; }
        public string PaymentMode { get; set; }


        public string? Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }



}
