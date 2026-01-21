using System.Security.Claims;
using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class FirmTermsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FirmTermsController> _logger;

        public FirmTermsController(
            ApplicationDbContext context,
            ILogger<FirmTermsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================================================
        // GET FIRM ID FROM TOKEN
        // =====================================================
        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            if (int.TryParse(firmIdStr, out var firmId))
                return firmId;

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetFirmTerms()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var terms = await _context.FirmTerms
                    .Include(ft => ft.Firm)
                    .Where(ft =>
                        ft.FirmId == firmId &&
                        !ft.IsDeleted)
                    .OrderByDescending(ft => ft.CreatedAt)
                    .Select(ft => new FirmTermResponseDto
                    {
                        FirmTermId = ft.FirmTermId,
                        FirmId = ft.FirmId,
                        FirmName = ft.Firm.FirmName,

                        Description = ft.Description,
                        IsActive = ft.IsActive,
                        CreatedAt = ft.CreatedAt,
                        UpdatedAt = ft.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Firm terms fetched successfully", terms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching firm terms");
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFirmTerm(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var term = await _context.FirmTerms
                    .Include(ft => ft.Firm)
                    .Where(ft =>
                        ft.FirmTermId == id &&
                        ft.FirmId == firmId &&
                        !ft.IsDeleted)
                    .Select(ft => new FirmTermResponseDto
                    {
                        FirmTermId = ft.FirmTermId,
                        FirmId = ft.FirmId,
                        FirmName = ft.Firm.FirmName,

                        Description = ft.Description,
                        IsActive = ft.IsActive,
                        CreatedAt = ft.CreatedAt,
                        UpdatedAt = ft.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (term == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                return ApiResponse(true, "Firm term fetched successfully", term);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching firm term {FirmTermId}", id);
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // =====================================================
        // CREATE FIRM TERM
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> CreateFirmTerm([FromBody] CreateFirmTermDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList(),
                    statusCode: 400);
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                bool duplicate = await _context.FirmTerms.AnyAsync(ft =>
                    ft.FirmId == firmId &&
                    ft.Description.ToLower() == dto.Description.ToLower() &&
                    !ft.IsDeleted);

                if (duplicate)
                    return ApiResponse(false,
                        "Firm term already exists",
                        error: "Duplicate",
                        statusCode: 409);

                var term = new FirmTerm
                {
                    FirmId = firmId.Value,   // 🔥 FROM TOKEN
                    Description = dto.Description.Trim(),
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.FirmTerms.Add(term);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm term created successfully", new
                {
                    term.FirmTermId
                }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating firm term");
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // =====================================================
        // UPDATE FIRM TERM
        // =====================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFirmTerm(int id, [FromBody] UpdateFirmTermDto dto)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var term = await _context.FirmTerms
                    .Where(ft =>
                        ft.FirmTermId == id &&
                        ft.FirmId == firmId &&
                        !ft.IsDeleted)
                    .FirstOrDefaultAsync();

                if (term == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                string newDescription = dto.Description.Trim();

                bool duplicate = await _context.FirmTerms.AnyAsync(ft =>
                    ft.FirmId == firmId &&
                    ft.FirmTermId != id &&
                    ft.Description.ToLower() == newDescription.ToLower() &&
                    !ft.IsDeleted);

                if (duplicate)
                    return ApiResponse(false,
                        "Firm term already exists",
                        error: "Duplicate",
                        statusCode: 409);

                term.Description = newDescription;
                term.IsActive = dto.IsActive;
                term.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm term updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating firm term {FirmTermId}", id);
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // =====================================================
        // DELETE FIRM TERM (SOFT DELETE)
        // =====================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFirmTerm(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var term = await _context.FirmTerms
                    .Where(ft =>
                        ft.FirmTermId == id &&
                        ft.FirmId == firmId &&
                        !ft.IsDeleted)
                    .FirstOrDefaultAsync();

                if (term == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                term.IsDeleted = true;
                term.IsActive = false;
                term.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm term deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting firm term {FirmTermId}", id);
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }
    }
}
