using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class FirmsController : BaseApiController
    {
        private readonly ApplicationDbContext _context; 
        public FirmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // GET ALL FIRMS
        // ================================
        [HttpGet]
        public async Task<IActionResult> GetFirms()
        {
            try
            {
                var firm = await _context.Firms
                    .Where(f => !f.IsDeleted)
                    .OrderBy(f => f.FirmName)
                    .Select(f => new FirmResponseDto
                    {
                        FirmName = f.FirmName,
                        IsActive = f.IsActive,
                        IsDeleted = f.IsDeleted
                    })
                    .ToListAsync();

                return ApiResponse(true, "Firm retrieved successfully",firm );
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving firms", error: ex.Message);
            }
        }

        // ================================
        // CREATE FIRM
        // ================================
        [HttpPost]

        public async Task<IActionResult> CreateFirm([FromForm] FirmCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ApiResponse(false, "Invalid data", null, "bad_request", errors);
            }

            try
            {
                bool exists = await _context.Firms.AnyAsync(f =>
                    f.FirmName.ToLower() == dto.FirmName.ToLower() && !f.IsDeleted);

                if (exists)
                    return ApiResponse(false, "Firm already exists", error: "Duplicate");


                var firm = new Firm
                {
                    FirmCode = dto.FirmCode,
                    FirmName = dto.FirmName.Trim(),
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Firms.Add(firm);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm created successfully", new
                {
                    firm.FirmId,
                    firm.FirmName
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error creating firm", error: ex.Message);
            }
        }

        // ================================
        // UPDATE FIRM
        // ================================
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateFirm(
            int id,
            [FromBody] FirmUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ApiResponse(false, "Invalid data", null, "bad_request", errors);
            }

            try
            {
                var firm = await _context.Firms
                    .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

                if (firm == null)
                    return ApiResponse(false, "Firm not found", error: "NotFound");

                bool duplicate = await _context.Firms.AnyAsync(f =>
                    f.FirmName.ToLower() == dto.FirmName.ToLower()
                    && f.FirmId != id
                    && !f.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Firm already exists", error: "Duplicate");

                firm.FirmName = dto.FirmName.Trim();
                firm.IsActive = dto.IsActive;
                firm.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error updating firm", error: ex.Message);
            }
        }
    }
}
