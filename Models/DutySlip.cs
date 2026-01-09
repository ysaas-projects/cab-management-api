using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    [Table("DutySlips")]
    public class DutySlip
    {
        [Key]
        public int DutySlipId { get; set; }

        public DateTime BookedDate { get; set; }

        public int BookedBy { get; set; }

        public int FirmId { get; set; }

        public int CustomerId { get; set; }

        public DateTime? ReportingDateTime { get; set; }

        public string? ReportingAddress { get; set; }

        public string? ReportingGeolocation { get; set; }

        public string? RequestedCab { get; set; }

        public string? SentCab { get; set; }

        public string? CabNumber { get; set; }

        public int? DriverId { get; set; }

        public string? PaymentMode { get; set; }

        public double? StartKms { get; set; }

        public string? StartKmsImagePath { get; set; }

        public DateTime? StartDateTime { get; set; }

        public double? CloseKms { get; set; }

        public string? CloseKmsImagePath { get; set; }

        public DateTime? CloseDateTime { get; set; }

        public double? TotalKms { get; set; }

        public int? TotalTimeInMin { get; set; }

        public string? NextDayInstruction { get; set; }

        public string? Destination { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; }

    }
    // ==============================
    // Create DutySlip Dto
    // ==============================
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

        public string RequestedCab { get; set; }

        public string Destination { get; set; }


    }

    // ==============================
    // Update AssignDriver Dto
    // ==============================
    public class AssignDriverDto
    {
        [Required]
        public int DriverId { get; set; }
        public string? CabNumber { get; set; }

        [Required]
        public DateTime ReportingDateTime { get; set; }

        public string ReportingAddress { get; set; }

        public string SentCab { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

    // ==============================
    // Update StartJourney Dto
    // ==============================
    public class StartJourneyDto
    {
        public string ReportingGeoLocation { get; set; }

        public decimal StartKms { get; set; }

        public IFormFile? StartKmsImagePath { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // ==============================
    // Update EndJourney Dto
    // ==============================
    public class EndJourneyDto
    {
        public decimal CloseKms { get; set; }

        public IFormFile? CloseKmsImagePath { get; set; }

        [Required]
        public DateTime CloseDateTime { get; set; }

        public decimal TotalKms { get; set; }

        public int TotalTimeInMin { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // ==============================
    // Update Instruction Dto
    // ==============================
    public class InstructionDto
    {
        public string NextDayInstruction { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // ==============================
    // Update Billing Dto
    // ==============================
    public class BillingDto
    {
        public string PaymentMode { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

}












