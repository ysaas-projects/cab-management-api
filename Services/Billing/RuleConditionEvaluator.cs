using cab_management.Models;
using System.Text.Json;

namespace cab_management.Services.Billing
{
	public static class RuleConditionEvaluator
	{
		public static bool Matches(string conditionJson, BillingContext ctx)
		{
			using var doc = JsonDocument.Parse(conditionJson);
			var when = doc.RootElement.GetProperty("when");

			if (when.TryGetProperty("always", out var always) && always.GetBoolean())
				return true;

			if (when.TryGetProperty("total_km_gt", out var km))
				if (!(ctx.TotalKm > km.GetDecimal())) return false;

			if (when.TryGetProperty("total_hr_gt", out var hr))
				if (!(ctx.TotalHours > hr.GetDecimal())) return false;

			if (when.TryGetProperty("trip_type", out var trip))
				if (!ctx.TripType.Equals(trip.GetString(), StringComparison.OrdinalIgnoreCase))
					return false;

			return true;
		}
	}
}
