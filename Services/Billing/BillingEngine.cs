using cab_management.Models;
using System.Text.Json;


namespace cab_management.Services.Billing
{
	public class BillingEngine
	{
		public List<BillLineResult> GenerateBill(
			BillingContext ctx,
			List<PricingRuleEntity> rules)
		{
			var billLines = new List<BillLineResult>();
			decimal subTotal = 0;

			foreach (var rule in rules.OrderBy(r => r.Priority))
			{
				if (!RuleConditionEvaluator.Matches(rule.ConditionJson, ctx))
					continue;

				var line = ApplyRule(rule, ctx, subTotal);
				if (line == null) continue;

				billLines.Add(line);

				if (rule.RuleCategory != "Tax")
					subTotal += line.Amount;
			}

			return billLines;
		}

		// 🔧 RULE CALCULATION LOGIC → STAYS HERE
		private BillLineResult? ApplyRule(
			PricingRuleEntity rule,
			BillingContext ctx,
			decimal subTotal)
		{
			decimal qty = 0;
			decimal amount = 0;

			switch (rule.CalculationType)
			{
				case "Fixed":
					qty = 1;
					amount = rule.RateValue;
					break;

				case "PerKm":
					qty = ctx.TotalKm - GetAfterValue(rule, "after_km");
					if (qty <= 0) return null;
					amount = qty * rule.RateValue;
					break;

				case "PerHour":
					qty = ctx.TotalHours - GetAfterValue(rule, "after_hr");
					if (qty <= 0) return null;
					amount = qty * rule.RateValue;
					break;

				case "PerDay":
					qty = ctx.TotalDays;
					amount = qty * rule.RateValue;
					break;

				case "Percentage":
					amount = (subTotal * rule.RateValue) / 100;
					qty = rule.RateValue;
					break;
			}

			return new BillLineResult
			{
				PricingRuleId = rule.PricingRuleId,
				RuleName = rule.RuleName,
				RuleCategory = rule.RuleCategory,
				Quantity = qty,
				Rate = rule.RateValue,
				Amount = Math.Round(amount, 2)
			};
		}

		private decimal GetAfterValue(PricingRuleEntity rule, string key)
		{
			using var doc = JsonDocument.Parse(rule.ConditionJson);

			if (doc.RootElement
				.GetProperty("apply")
				.TryGetProperty(key, out var val))
			{
				return val.GetDecimal();
			}

			return 0;
		}
	}
}
