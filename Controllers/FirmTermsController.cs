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
    [Route("api/[controller]")]
    [ApiController]
    public class FirmTermsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public FirmTermsController(ApplicationDbContext context)
        {
            _context = context;  
        }


        // ------------------------------
        // GET ALL
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> GetFirmTerms()
        {
            var terms = await _context.FirmTerms
                .Where(ft => !ft.IsDeleted)
                .OrderByDescending(ft => ft.CreatedAt)
                .ToListAsync();

            return ApiResponse(true, "Firm terms retrieved successfully", terms);
        }

        // ------------------------------
        // GET BY ID
        // ------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFirmTermById(int id)
        {
            var term = await _context.FirmTerms
                .FirstOrDefaultAsync(ft => ft.FirmTermId == id && !ft.IsDeleted);

            if (term == null)
                return ApiResponse(false, "Firm term not found", error: "NotFound");

            return ApiResponse(true, "Firm term retrieved successfully", term);
        }

        // ==============================
        // CREATE
        // ==============================
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme + "," +
            CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateFirmTerm([FromBody] CreateFirmTermDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                // Check firm exists
                bool firmExists = await _context.Firms
                    .AnyAsync(f => f.FirmId == dto.FirmId && !f.IsDeleted);

                if (!firmExists)
                    return ApiResponse(false, "Invalid FirmId", error: "BadRequest");

                // Duplicate check
                bool duplicate = await _context.FirmTerms.AnyAsync(ft =>
                    ft.FirmId == dto.FirmId &&
                    ft.Description.ToLower() == dto.Description.ToLower() &&
                    !ft.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Firm term already exists for this firm", error: "Duplicate");

                var firmTerm = new FirmTerms
                {
                    FirmId = dto.FirmId,
                    Description = dto.Description.Trim(),
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.FirmTerms.Add(firmTerm);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm term created successfully", new
                {
                    firmTerm.FirmTermId,
                    firmTerm.FirmId,
                    firmTerm.Description,
                    firmTerm.IsActive
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // ==============================
        // UPDATE
        // ==============================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFirmTerm(int id, [FromBody] UpdateFirmTermDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                var term = await _context.FirmTerms
                    .FirstOrDefaultAsync(ft => ft.FirmTermId == id && !ft.IsDeleted);

                if (term == null)
                    return ApiResponse(false, "Firm term not found", error: "NotFound");

                // Correct duplicate check
                string newDescription = model.Description.Trim();

                bool duplicate = await _context.FirmTerms
                    .AnyAsync(ft =>
                        ft.FirmId == term.FirmId &&
                        ft.FirmTermId != id &&
                        ft.Description.Trim().ToLower() == newDescription.ToLower() &&
                        !ft.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Firm term already exists for this firm", error: "Duplicate");

                term.Description = newDescription;
                term.IsActive = model.IsActive;
                term.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Firm term updated successfully", new
                {
                    term.FirmTermId,
                    term.FirmId,
                    term.Description,
                    term.IsActive
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }


        // ==============================
        // DELETE (SOFT DELETE)
        // ==============================
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme + "," +
            CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFirmTerm(int id)
        {
            var term = await _context.FirmTerms
                .FirstOrDefaultAsync(ft => ft.FirmTermId == id && !ft.IsDeleted);

            if (term == null)
                return ApiResponse(false, "Firm term not found", error: "NotFound");

            term.IsDeleted = true;
            term.IsActive = false;
            term.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Firm term deleted successfully");
        }

        // ------------------------------
        // GET BY FIRM ID
        // ------------------------------
        [HttpGet("firm/{firmId}")]
        public async Task<IActionResult> GetFirmTermsByFirmId(int firmId)
        {
            var terms = await _context.FirmTerms
                .Where(ft => ft.FirmId == firmId && !ft.IsDeleted)
                .OrderByDescending(ft => ft.CreatedAt)
                .ToListAsync();

            return ApiResponse(true, "Firm terms retrieved successfully", terms);
        }

    }

    
}
