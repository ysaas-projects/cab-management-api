using cab_management.Data;
using cab_management.Models;
using cab_management.Services.Billing;
using Microsoft.EntityFrameworkCore;


public class BillingService : IBillingService
{
	private readonly ApplicationDbContext _context;
	private readonly BillingEngine _engine;

	public BillingService(ApplicationDbContext context)
	{
		_context = context;
		_engine = new BillingEngine();
	}

	public async Task<List<BillLineResult>> GenerateBillAsync(int dutySlipId)
	{
		var dutySlip = await _context.DutySlips.FindAsync(dutySlipId);
		if (dutySlip == null)
			throw new Exception("Duty slip not found");

		var ctx = BuildContext(dutySlip);

		var rules = await LoadPricingRules(dutySlip.FirmId);

		return _engine.GenerateBill(ctx, rules);
	}

	private BillingContext BuildContext(DutySlip d)
	{
		return new BillingContext
		{
			DutySlipId = d.DutySlipId,
			TotalKm = d.TotalKms ?? 0,
			TotalMinutes = d.TotalTimeInMin ?? 0,
			TotalHours = (d.TotalTimeInMin ?? 0) / 60m,
			TotalDays = (int)Math.Ceiling(((d.TotalTimeInMin ?? 0) / 60m) / 24m),
			TripType = d.Destination != null ? "Outstation" : "Local",
			Status = d.Status ?? "Completed",
			StartDateTime = d.StartDateTime ?? DateTime.Now,
			CloseDateTime = d.CloseDateTime ?? DateTime.Now
		};
	}

	private async Task<List<PricingRuleEntity>> LoadPricingRules(int firmId)
	{
		return await _context.PricingRules2
			.Where(r => r.FirmId == firmId && r.IsActive)
			.OrderBy(r => r.Priority)
			.Select(r => new PricingRuleEntity
			{
				PricingRuleId = r.PricingRuleId,
				RuleName = r.RuleName,
				RuleCategory = r.RuleCategory,
				CalculationType = r.CalculationType,
				ConditionJson = r.ConditionJson,
				RateValue = r.RateValue,
				Priority = r.Priority
			})
			.ToListAsync();
	}


	private string GenerateBillNumber(int firmId)
	{
		return $"BILL-{firmId}-{DateTime.Now:yyyyMMddHHmmss}";
	}

	public async Task<int> GenerateAndSaveBillAsync(int dutySlipId)
	{
		using var tx = await _context.Database.BeginTransactionAsync();

		try
		{

			// 🔍 Check existing bill
			var existingBill = await _context.DutySlipBills
				.Where(b =>
					b.DutySlipId == dutySlipId &&
					!b.IsDeleted)
				.OrderByDescending(b => b.BillId)
				.FirstOrDefaultAsync();

			if (existingBill != null)
			{
				if (existingBill.BillStatus == BillStatus.Final)
				{
					throw new InvalidOperationException(
						$"Final bill already exists (Bill No: {existingBill.BillNumber}). Cancel it before regenerating."
					);
				}

				// 🧹 Soft-delete old draft / cancelled bill
				existingBill.IsDeleted = true;
				existingBill.UpdatedAt = DateTime.Now;

				var oldDetails = await _context.DutySlipBillDetails
					.Where(d => d.BillId == existingBill.BillId)
					.ToListAsync();

				_context.DutySlipBillDetails.RemoveRange(oldDetails);
				await _context.SaveChangesAsync();
			}



			var dutySlip = await _context.DutySlips.FindAsync(dutySlipId);
			if (dutySlip == null)
				throw new Exception("DutySlip not found");

			// 1. Generate bill lines (already built earlier)
			var ctx = BuildContext(dutySlip);
			var rules = await LoadPricingRules(dutySlip.FirmId);

			var billLines = _engine.GenerateBill(ctx, rules);

			// 2. Calculate totals
			var subTotal = billLines
				.Where(x => x.RuleCategory != "Tax")
				.Sum(x => x.Amount);

			var gstLine = billLines.FirstOrDefault(x => x.RuleCategory == "Tax");
			var gstAmount = gstLine?.Amount ?? 0;

			var grandTotal = subTotal + gstAmount;

			// 3. Create bill header
			var bill = new DutySlipBill
			{
				DutySlipId = dutySlip.DutySlipId,
				FirmId = dutySlip.FirmId,
				CustomerId = dutySlip.CustomerId,

				BillNumber = GenerateBillNumber(dutySlip.FirmId),
				BillDate = DateTime.Now,

				SubTotal = subTotal,
				GstPercentage = gstLine?.Rate ?? 0,
				GstAmount = gstAmount,

				RoundOffAmount = Math.Round(grandTotal) - grandTotal,
				GrandTotal = Math.Round(grandTotal),

				BillStatus = "Draft",
				CreatedAt = DateTime.Now
			};

			_context.DutySlipBills.Add(bill);
			await _context.SaveChangesAsync();

			// 4. Save bill details
			var details = billLines.Select(x => new DutySlipBillDetail
			{
				BillId = bill.BillId,
				PricingRuleId = x.PricingRuleId,
				RuleName = x.RuleName,
				RuleCategory = x.RuleCategory,
				Quantity = x.Quantity,
				Rate = x.Rate,
				Amount = x.Amount,
				CreatedAt = DateTime.Now
			}).ToList();

			_context.DutySlipBillDetails.AddRange(details);
			await _context.SaveChangesAsync();

			await tx.CommitAsync();

			return bill.BillId;
		}
		catch
		{
			await tx.RollbackAsync();
			throw;
		}
	}

	public async Task<BillPreviewResponse> PreviewBillAsync(int dutySlipId)
	{
		var dutySlip = await _context.DutySlips.FindAsync(dutySlipId);
		if (dutySlip == null)
			throw new Exception("DutySlip not found");

		// 1. Build context
		var ctx = BuildContext(dutySlip);

		// 2. Load rules
		var rules = await LoadPricingRules(dutySlip.FirmId);

		// 3. Generate bill lines
		var billLines = _engine.GenerateBill(ctx, rules);

		// 4. Totals
		var subTotal = billLines
			.Where(x => x.RuleCategory != "Tax")
			.Sum(x => x.Amount);

		var gstLine = billLines.FirstOrDefault(x => x.RuleCategory == "Tax");

		var gstAmount = gstLine?.Amount ?? 0;
		var gstPercent = gstLine?.Rate ?? 0;

		var grandTotal = subTotal + gstAmount;

		return new BillPreviewResponse
		{
			DutySlipId = dutySlipId,
			SubTotal = subTotal,
			GstPercentage = gstPercent,
			GstAmount = gstAmount,
			GrandTotal = Math.Round(grandTotal),
			Lines = billLines
		};
	}

	public async Task FinalizeBillAsync(int billId)
	{
		var bill = await _context.DutySlipBills
			.Where(b => b.BillId == billId && !b.IsDeleted)
			.FirstOrDefaultAsync();

		if (bill == null)
			throw new Exception("Bill not found");

		if (bill.BillStatus == BillStatus.Final)
			throw new InvalidOperationException("Bill is already finalized");

		if (bill.BillStatus == BillStatus.Cancelled)
			throw new InvalidOperationException("Cancelled bill cannot be finalized");

		// 🔒 Finalize bill
		bill.BillStatus = BillStatus.Final;
		bill.UpdatedAt = DateTime.Now;

		// 🔒 LOCK DUTY SLIP BILLING
		var dutySlip = await _context.DutySlips.FindAsync(bill.DutySlipId);
		if (dutySlip != null)
		{
			dutySlip.IsBillingLocked = true;
		}

		await _context.SaveChangesAsync();
	}

	public async Task CancelBillAsync(int billId, int cancelledBy, string reason)
	{
		var bill = await _context.DutySlipBills
			.Where(b => b.BillId == billId && !b.IsDeleted)
			.FirstOrDefaultAsync();

		if (bill == null)
			throw new Exception("Bill not found");

		if (bill.BillStatus == BillStatus.Cancelled)
			throw new InvalidOperationException("Bill is already cancelled");

		// 🔒 Final bills CAN be cancelled (real-world requirement)
		bill.BillStatus = BillStatus.Cancelled;
		bill.CancelReason = reason;
		bill.CancelledBy = cancelledBy;
		bill.CancelledAt = DateTime.Now;
		bill.UpdatedAt = DateTime.Now;

		// 🔓 UNLOCK DUTY SLIP
		var dutySlip = await _context.DutySlips.FindAsync(bill.DutySlipId);
		if (dutySlip != null)
		{
			dutySlip.IsBillingLocked = false;
		}

		await _context.SaveChangesAsync();
	}





}
