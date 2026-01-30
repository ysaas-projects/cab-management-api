using cab_management.Data;
using cab_management.Models;
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
	[Route("api/[controller]")]
    [ApiController]
    public class DutyExpenseController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public DutyExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

		private int? GetFirmIdFromToken()
		{
			var firmIdStr = User.FindFirstValue("firmId");
			return int.TryParse(firmIdStr, out var firmId) ? firmId : null;
		}

		private int GetUserIdFromToken()
		{
			return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
		}

		// =====================================================
		// GET ALL DutyExpense
		// =====================================================
		[HttpGet]
        public async Task<IActionResult> GetAllDutyExpense()
        {
            try
            {
				var firmId = GetFirmIdFromToken();
				if (firmId == null)
					return ApiResponse(false, "Unauthorized", statusCode: 401);

				var dutyExpense = await _context.DutyExpenses
                    .Where(d => d.IsDeleted != true && d.FirmId == firmId)
                    .ToListAsync();

                return ApiResponse(true, "DutyExpense retrieved successfully", dutyExpense);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
        // =====================================================
        // GET DutyExpense BY ID
        // =====================================================
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdDutyExpense(int id)
        {
            try
            {

				var firmId = GetFirmIdFromToken();
				if (firmId == null)
					return ApiResponse(false, "Unauthorized", statusCode: 401);

				var d = await _context.DutyExpenses
                    .FirstOrDefaultAsync(x => x.DutyExpenseId == id && x.IsDeleted == false && x.FirmId == firmId);

                if (d != null)
                {
                    return ApiResponse(true, "DutyExpense retrieved successfully", d);
                }
                else
                {
                    return ApiResponse(false, "Record not found", 404);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
        // =====================================================
        // CREATE DutyExpense
        // =====================================================
        [HttpPost]

        public async Task<IActionResult> CreateDutyExpense([FromBody] CreateDutyExpenseDTO dto)
        {
            try
            {
				var firmId = GetFirmIdFromToken();
				if (firmId == null)
					return ApiResponse(false, "Unauthorized", statusCode: 401);

				if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Validation failed", errors);
                }

                DutyExpense dutyExpense = new DutyExpense
                {
                    FirmId = firmId,
					DutyId = dto.DutyId,
                    ExpenseType = dto.ExpenseType,
                    Description = dto.Description,
                    ExpenseAmount = dto.ExpenseAmount,
                    CreatedAt = DateTime.Now
                };

                await _context.DutyExpenses.AddAsync(dutyExpense);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Data added successfully", dutyExpense);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
        // =====================================================
        // UPDATE DutyExpense BY ID
        // =====================================================
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDutyExpenseById(int id, [FromBody] UpdateDutyExpenseDTO dto)
        {
            try
            {
				var firmId = GetFirmIdFromToken();
				if (firmId == null)
					return ApiResponse(false, "Unauthorized", statusCode: 401);

				var d = await _context.DutyExpenses.FindAsync(id);

                if (d == null || d.IsDeleted == true || d.FirmId != firmId)
                {
                    return ApiResponse(false, "Record not found", 404);
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Validation failed", errors);
                }

                d.DutyId = dto.DutyId;
                d.ExpenseType = dto.ExpenseType;
                d.Description = dto.Description;
                d.ExpenseAmount = dto.ExpenseAmount;
                d.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "DutyExpense updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
        // =====================================================
        // DELETE DutyExpense BY ID (Soft Delete)
        // =====================================================
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteDutyExpenseById(int id)
        {
            try
            {

				var firmId = GetFirmIdFromToken();
				if (firmId == null)
					return ApiResponse(false, "Unauthorized", statusCode: 401);

				var d = await _context.DutyExpenses
                    .FirstOrDefaultAsync(e => e.DutyExpenseId == id && e.IsDeleted == false && e.FirmId == firmId);

                if (d == null)
                {
                    return ApiResponse(false, "Record not found", 400);
                }

                d.IsDeleted = true;
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Record deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
    }
}