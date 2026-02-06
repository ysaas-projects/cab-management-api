using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{

	[Table("PricingRules2")]
	public class PricingRules2
	{
		[Key] 
		public int PricingRuleId { get; set; }
		public int FirmId { get; set; }

		public string RuleName { get; set; }
		public string RuleCategory { get; set; }
		public string CalculationType { get; set; }

		public string ConditionJson { get; set; }
		public decimal RateValue { get; set; }

		public int Priority { get; set; }
		public bool IsActive { get; set; }
	}

	public class PricingRule2Entity
	{
		public int PricingRuleId { get; set; }
		public string RuleName { get; set; }
		public string RuleCategory { get; set; }
		public string CalculationType { get; set; }
		public string ConditionJson { get; set; }
		public decimal RateValue { get; set; }
		public int Priority { get; set; }
	}


}
