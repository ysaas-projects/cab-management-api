using cab_management.Data;
using cab_management.Models;
using cab_management.Services.Billing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class DutySlipsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DutySlipsController> _logger;
        private readonly IBillingService _billingService;


		public DutySlipsController(
            ApplicationDbContext context,
            ILogger<DutySlipsController> logger,
            IBillingService billingService)
        {
            _context = context;
            _logger = logger;
            _billingService = billingService;
        }

        // ================= GET FirmId from JWT =================
        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            return int.TryParse(firmIdStr, out var firmId) ? firmId : null;
        }

        private int GetUserIdFromToken()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }


		private IActionResult? CheckBillingLock(DutySlip dutySlip)
		{
			if (dutySlip.IsBillingLocked)
			{
				return ApiResponse(false,
					"Billing is finalized. Duty slip cannot be modified.",
					statusCode: 409);
			}
			return null;
		}

        //var lockResult = CheckBillingLock(dutySlip);
        //if (lockResult != null) return lockResult;


		// ================= CREATE DUTY SLIP =================
		[HttpPost]
        public async Task<IActionResult> CreateDutySlip([FromBody] CreateDutySlipDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList(),
                    statusCode: 400);

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Unauthorized", statusCode: 401);
                var userId = GetUserIdFromToken(); // ✅ FROM TOKEN

                var dutySlip = new DutySlip
                {
                    BookedDate = dto.BookedDate,
                    BookedBy = userId,              // ✅ TOKEN USER
                    FirmId = firmId.Value,
                    CustomerId = dto.CustomerId,
                    RequestedCab = dto.RequestedCab,
                    Destination = dto.Destination,
                    Status = "Booked",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.DutySlips.Add(dutySlip);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Duty slip created successfully", dutySlip, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating duty slip");
                return ApiResponse(false, "Error creating duty slip", error: ex.Message, statusCode: 500);
            }
        }

        // ================= ASSIGN DRIVER =================
        [HttpPut("{id}/assign-driver")]
        public async Task<IActionResult> AssignDriver(int id, [FromBody] UpdateAssignDriverDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", statusCode: 401);



            var dutySlip = await _context.DutySlips
                .FirstOrDefaultAsync(x => x.DutySlipId == id && x.FirmId == firmId && !x.IsDeleted);

            if (dutySlip == null)
                return ApiResponse(false, "Duty slip not found", statusCode: 404);

			// 🔒 ADD THIS
			if (dutySlip.IsBillingLocked)
			{
				return ApiResponse(false,
					"Billing is finalized. Cannot modify duty slip.",
					statusCode: 409);
			}

			dutySlip.DriverDetailId = dto.DriverDetailId;
            dutySlip.ReportingAddress = dto.ReportingAddress;
            dutySlip.ReportingDateTime = dto.ReportingDateTime;
            dutySlip.SentCab = dto.SentCab;
            dutySlip.CabNumber = dto.CabNumber;
            dutySlip.Status = "Driver-Assigned";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Driver assigned successfully");
        }

        // ================= START JOURNEY =================
        [HttpPut("{id}/start-journey")]
        public async Task<IActionResult> StartJourney(int id, [FromBody] UpdateStartJourneyDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", statusCode: 401);

            var dutySlip = await _context.DutySlips
                .FirstOrDefaultAsync(x => x.DutySlipId == id && x.FirmId == firmId && !x.IsDeleted);

            if (dutySlip == null)
                return ApiResponse(false, "Duty slip not found", statusCode: 404);

			// 🔒 ADD THIS
			if (dutySlip.IsBillingLocked)
			{
				return ApiResponse(false,
					"Billing is finalized. Journey data cannot be modified.",
					statusCode: 409);
			}

			dutySlip.ReportingGeoLocation = dto.ReportingGeoLocation;
            dutySlip.StartKms = dto.StartKms;
            dutySlip.StartKmsImagePath = dto.StartKmsImagePath;
            dutySlip.StartDateTime = dto.StartDateTime;
            dutySlip.Status = "Start-Journey";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Journey started successfully");
        }

        // ================= END JOURNEY =================
        [HttpPut("{id}/end-journey")]
        public async Task<IActionResult> EndJourney(int id, [FromBody] UpdateEndJourneyDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", statusCode: 401);

            var dutySlip = await _context.DutySlips
                .FirstOrDefaultAsync(x => x.DutySlipId == id && x.FirmId == firmId && !x.IsDeleted);

            if (dutySlip == null)
                return ApiResponse(false, "Duty slip not found", statusCode: 404);

			if (dutySlip.IsBillingLocked)
			{
				return ApiResponse(false,
					"Billing is finalized. KM and time cannot be changed.",
					statusCode: 409);
			}

			dutySlip.CloseKms = dto.CloseKms;
            dutySlip.CloseKmsImagePath = dto.CloseKmsImagePath;
            dutySlip.CloseDateTime = dto.CloseDateTime;
            dutySlip.TotalKms = dto.TotalKms;
            dutySlip.TotalTimeInMin = dto.TotalTimeInMin;
            dutySlip.Status = "End-Journey";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Journey ended successfully");
        }

        // ================= INSTRUCTION =================
        [HttpPut("{id}/instruction")]
        public async Task<IActionResult> UpdateInstruction(int id, [FromBody] UpdateInstructionDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", statusCode: 401);

            var dutySlip = await _context.DutySlips
                .FirstOrDefaultAsync(x => x.DutySlipId == id && x.FirmId == firmId && !x.IsDeleted);

            if (dutySlip == null)
                return ApiResponse(false, "Duty slip not found", statusCode: 404);

            dutySlip.NextDayInstruction = dto.NextDayInstruction;
            dutySlip.Status = "Instructed";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Instruction updated successfully");
        }

        // ================= BILLING =================
        [HttpPut("{id}/billing")]
        public async Task<IActionResult> Billing(int id, [FromBody] UpdateBillingDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", statusCode: 401);

            var dutySlip = await _context.DutySlips
                .FirstOrDefaultAsync(x => x.DutySlipId == id && x.FirmId == firmId && !x.IsDeleted);

            if (dutySlip == null)
                return ApiResponse(false, "Duty slip not found", statusCode: 404);

			if (dutySlip.IsBillingLocked)
			{
				return ApiResponse(false,
					"Billing is finalized. Payment details cannot be changed.",
					statusCode: 409);
			}

			dutySlip.PaymentMode = dto.PaymentMode;
            dutySlip.Status = "Bill-Pending";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Billing updated successfully");
        }

        // ================= GET ALL DUTY SLIPS (Firm-wise) =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", statusCode: 401);

            var dutySlips = await _context.DutySlips
                .Include(x => x.Firm)
                .Include(x => x.Customer)
                .Include(x => x.DriverDetail)
                .Include(x => x.RequestedCabNav)
                .Include(x => x.SentCabNav)
                .Where(x => x.FirmId == firmId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new DutySlipResponseDto
                {
                    DutySlipId = x.DutySlipId,

                    BookedDate = x.BookedDate,
                    BookedBy = x.BookedBy,

                    FirmId = x.FirmId,
                    FirmName = x.Firm.FirmName,

                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.CustomerName,

                    DriverDetailId = x.DriverDetailId,
                    DriverName = x.DriverDetail != null ? x.DriverDetail.DriverName : null,

                    RequestedCab = x.RequestedCab,
                    RequestedCabType = x.RequestedCabNav != null
                        ? x.RequestedCabNav.CabType
                        : null,

                    SentCab = x.SentCab,
                    SentCabType = x.SentCabNav != null
                        ? x.SentCabNav.CabType
                        : null,
                    StartKms = x.StartKms,
                    StartDateTime = x.StartDateTime,
                    Destination = x.Destination,
                    Status = x.Status,

                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return ApiResponse(true, "Duty slips fetched successfully", dutySlips);
        }


		// ================= GET DUTY SLIP + EXPENSES =================
		[HttpGet("{dutySlipId}/details")]
		public async Task<IActionResult> GetDutySlipWithExpenses(int dutySlipId)
		{
			var firmId = GetFirmIdFromToken();
			if (firmId == null)
				return ApiResponse(false, "Unauthorized", statusCode: 401);

			// ------------------ DUTY SLIP ------------------
			var dutySlip = await _context.DutySlips
				.Include(x => x.Firm)
				.Include(x => x.Customer)
				.Include(x => x.DriverDetail)
				.Include(x => x.RequestedCabNav)
				.Include(x => x.SentCabNav)
				.Where(x =>
					x.DutySlipId == dutySlipId &&
					x.FirmId == firmId &&
					!x.IsDeleted
				)
				.Select(x => new DutySlipResponseDto
				{
					DutySlipId = x.DutySlipId,

					BookedDate = x.BookedDate,
					BookedBy = x.BookedBy,

					FirmId = x.FirmId,
					FirmName = x.Firm.FirmName,

					CustomerId = x.CustomerId,
					CustomerName = x.Customer.CustomerName,

					DriverDetailId = x.DriverDetailId,
					DriverName = x.DriverDetail != null
						? x.DriverDetail.DriverName
						: null,

					RequestedCab = x.RequestedCab,
					RequestedCabType = x.RequestedCabNav != null
						? x.RequestedCabNav.CabType
						: null,

					SentCab = x.SentCab,
					SentCabType = x.SentCabNav != null
						? x.SentCabNav.CabType
						: null,

					StartKms = x.StartKms,
					StartDateTime = x.StartDateTime,

                    CloseKms = x.CloseKms,
                    CloseDateTime = x.CloseDateTime,

                    TotalKms = x.TotalKms,
                    TotalTimeInMin = x.TotalTimeInMin,

					Destination = x.Destination,
					Status = x.Status,

					CreatedAt = x.CreatedAt
				})
				.FirstOrDefaultAsync();

			if (dutySlip == null)
				return ApiResponse(false, "Duty slip not found", statusCode: 404);

			// ------------------ EXPENSES ------------------
			var expenses = await _context.DutyExpenses
				.Where(x =>
					x.DutyId == dutySlipId &&
					x.FirmId == firmId &&
					!x.IsDeleted
				)
				.Select(x => new DutyExpenseDto
				{
					DutyExpenseId = x.DutyExpenseId,
					DutyId = x.DutyId,
					ExpenseType = x.ExpenseType,
					Description = x.Description,
					ExpenseAmount = x.ExpenseAmount,
					CreatedAt = x.CreatedAt
				})
				.ToListAsync();

			// ------------------ TOTAL ------------------
			decimal totalExpense = expenses.Sum(x =>
				decimal.TryParse(x.ExpenseAmount, out var amt)
					? amt
					: 0
			);

			// ------------------ RESPONSE ------------------
			var result = new DutySlipWithExpensesDto
			{
				DutySlip = dutySlip,
				Expenses = expenses,
				TotalExpenseAmount = totalExpense
			};

			return ApiResponse(true, "Duty slip details fetched", result);
		}



		[HttpPost("{dutySlipId}/generate-bill")]
		public async Task<IActionResult> GenerateBill(int dutySlipId)
		{
			try
			{
				var billId = await _billingService.GenerateAndSaveBillAsync(dutySlipId);

				return ApiResponse(true, "Bill generated successfully", new
				{
					BillId = billId
				});
			}
			catch (InvalidOperationException ex)
			{
				return ApiResponse(false, ex.Message, statusCode: 409);
			}
		}


		[HttpGet("{dutySlipId}/bill-preview")]
		public async Task<IActionResult> PreviewBill(int dutySlipId)
		{
			try
			{
				var preview = await _billingService.PreviewBillAsync(dutySlipId);
				return ApiResponse(true, "Bill preview generated", preview);
			}
			catch (Exception ex)
			{
				return ApiResponse(false, ex.Message, statusCode: 400);
			}
		}



		[HttpPost("finalize-bill/{billId}")]
		public async Task<IActionResult> FinalizeBill(int billId)
		{
			try
			{
				await _billingService.FinalizeBillAsync(billId);

				return ApiResponse(true, "Bill finalized successfully");
			}
			catch (InvalidOperationException ex)
			{
				return ApiResponse(false, ex.Message, statusCode: 409);
			}
			catch (Exception ex)
			{
				return ApiResponse(false, ex.Message, statusCode: 400);
			}
		}


		[HttpPost("cancel-bill/{billId}")]
		public async Task<IActionResult> CancelBill(
			int billId,
			[FromBody] CancelBillDto dto)
		{
			try
			{
				var userId = GetUserIdFromToken();

				await _billingService.CancelBillAsync(
					billId,
					userId,
					dto.Reason
				);

				return ApiResponse(true, "Bill cancelled successfully");
			}
			catch (InvalidOperationException ex)
			{
				return ApiResponse(false, ex.Message, statusCode: 409);
			}
			catch (Exception ex)
			{
				return ApiResponse(false, ex.Message, statusCode: 400);
			}
		}





	}
}
