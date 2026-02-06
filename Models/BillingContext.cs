namespace cab_management.Models
{
	public class BillingContext
	{
		public int DutySlipId { get; set; }

		public decimal TotalKm { get; set; }
		public decimal TotalHours { get; set; }
		public int TotalMinutes { get; set; }
		public int TotalDays { get; set; }

		public string TripType { get; set; } = "Local"; // Local / Outstation
		public string Status { get; set; } = "Completed";

		public DateTime StartDateTime { get; set; }
		public DateTime CloseDateTime { get; set; }
	}

	public class PricingRuleEntity
	{
		public int PricingRuleId { get; set; }
		public string RuleName { get; set; }
		public string RuleCategory { get; set; }
		public string CalculationType { get; set; }
		public string ConditionJson { get; set; }
		public decimal RateValue { get; set; }
		public int Priority { get; set; }
	}

	public class BillLineResult
	{
		public int PricingRuleId { get; set; }
		public string RuleName { get; set; }
		public string RuleCategory { get; set; } = null!;
		public decimal Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
	}

	public static class BillStatus
	{
		public const string Draft = "Draft";
		public const string Final = "Final";
		public const string Cancelled = "Cancelled";
	}

	public class BillPreviewResponse
	{
		public int DutySlipId { get; set; }

		public decimal SubTotal { get; set; }
		public decimal GstPercentage { get; set; }
		public decimal GstAmount { get; set; }

		public decimal GrandTotal { get; set; }

		public List<BillLineResult> Lines { get; set; } = new();
	}


}
