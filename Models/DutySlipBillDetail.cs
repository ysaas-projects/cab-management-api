using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
	public class DutySlipBillDetail
	{
		[Key]
		public int BillDetailId { get; set; }

		public int BillId { get; set; }
		public int PricingRuleId { get; set; }

		public string RuleName { get; set; } = null!;
		public string RuleCategory { get; set; } = null!;

		public decimal Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }

		public DateTime CreatedAt { get; set; }
	}

}
