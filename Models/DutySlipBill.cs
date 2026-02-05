using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
	public class DutySlipBill
	{
		[Key]
		public int BillId { get; set; }

		public int DutySlipId { get; set; }
		public int FirmId { get; set; }
		public int CustomerId { get; set; }

		public string BillNumber { get; set; } = null!;
		public DateTime BillDate { get; set; }

		public decimal SubTotal { get; set; }
		public decimal GstPercentage { get; set; }
		public decimal GstAmount { get; set; }

		public decimal RoundOffAmount { get; set; }
		public decimal GrandTotal { get; set; }

		public string BillStatus { get; set; } = "Draft";

		public DateTime CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		// 🔥 CANCEL AUDIT
		public string? CancelReason { get; set; }
		public int? CancelledBy { get; set; }
		public DateTime? CancelledAt { get; set; }

		public bool IsDeleted { get; set; } = false;
	}

	public class CancelBillDto
	{
		[Required]
		public string Reason { get; set; } = null!;
	}

}
