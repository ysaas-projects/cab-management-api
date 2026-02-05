using cab_management.Models;
using cab_management.Models;

namespace cab_management.Services.Billing
{
	public interface IBillingService
	{
		/// <summary>
		/// Generates bill calculation for a given DutySlip
		/// (Does NOT save to DB yet – only calculation)
		/// </summary>
		/// <param name="dutySlipId">DutySlip primary key</param>
		/// <returns>List of calculated bill line items</returns>
		Task<List<BillLineResult>> GenerateBillAsync(int dutySlipId);
		Task<int> GenerateAndSaveBillAsync(int dutySlipId);
		Task<BillPreviewResponse> PreviewBillAsync(int dutySlipId);

		Task FinalizeBillAsync(int billId);

		Task CancelBillAsync(int billId, int cancelledBy, string reason);



	}
}
