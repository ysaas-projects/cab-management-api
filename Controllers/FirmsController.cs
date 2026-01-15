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
                var firms = await _context.Firms
                    .Where(f => !f.IsDeleted)
                    .OrderBy(f => f.FirmName)
                    .Select(f => new FirmResponseDto
                    {
                        FirmId = f.FirmId,
                        FirmName = f.FirmName,
                        FirmCode = f.FirmCode,
                        IsActive = f.IsActive,

                        FirmDetails = f.FirmDetails
                            .Where(fd => !fd.IsDeleted)
                            .Select(fd => new FirmDetailsDto
                            {
                                Address = fd.Address,
                                ContactNumber = fd.ContactNumber,
                                ContactPerson = fd.ContactPerson,
                                GstNumber = fd.GstNumber,
                                LogoImagePath = fd.LogoImagePath,
                                IsActive = fd.IsActive
                            })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return ApiResponse(true, "Firm retrieved successfully", firms);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving firms", error: ex.Message);
            }
        }


        // ================================
        // GET FIRM BY ID
        // ================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFirmById(int id)
        {
            try
            {
                var firm = await _context.Firms
                    .Where(f => f.FirmId == id && !f.IsDeleted)
                    .Select(f => new
                    {
                        f.FirmId,
                        f.FirmName,
                        f.FirmCode,
                        f.IsActive,
                        FirmDetails = f.FirmDetails
                            .Where(fd => !fd.IsDeleted)
                            .Select(fd => new
                            {
                                fd.Address,
                                fd.ContactNumber,
                                fd.ContactPerson,
                                fd.GstNumber,
                                fd.LogoImagePath,
                                fd.IsActive
                            })
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                if (firm == null)
                {
                    return ApiResponse(false, "Firm not found", error: "NotFound");
                }

                return ApiResponse(true, "Firm retrieved successfully", firm);
            }
            catch (Exception ex)
            {

                return ApiResponse(
                    false,
                    "An error occurred while retrieving firm details",
                    error: ex.Message
                );
            }
        }


        // ================================
        // CREATE FIRM
        // ================================

        [HttpPost]
        public async Task<IActionResult> CreateFirm([FromForm] FirmCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ApiResponse(false, "Invalid data", null, "bad_request", errors);
            }

            bool exists = await _context.Firms.AnyAsync(f =>
                (f.FirmName.ToLower() == dto.FirmName.ToLower()
                || f.FirmCode.ToLower() == dto.FirmCode.ToLower())
                && !f.IsDeleted);

            if (exists)
                return ApiResponse(false, "Firm already exists", error: "Duplicate");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var firm = new Firm
                {
                    FirmName = dto.FirmName.Trim(),
                    FirmCode = dto.FirmCode.Trim(),
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Firms.Add(firm);
                await _context.SaveChangesAsync(); 

                var firmDetails = new FirmDetail
                {
                    FirmId = firm.FirmId,
                    Address="",
                    ContactNumber="",
                    ContactPerson="",
                    GstNumber="",
                    LogoImagePath="",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.FirmDetails.Add(firmDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return ApiResponse(true, "Firm and FirmDetails created successfully", new
                {
                    firm.FirmId,
                    firm.FirmName
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse(false, "Error creating firm", error: ex.Message);
            }
        }


        //================================
        //UPDATE FIRM
        //================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFirm(int id, [FromBody] FirmUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ApiResponse(false, "Invalid data", null, "bad_request", errors);
            }

            var firm = await _context.Firms
                .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

            if (firm == null)
                return ApiResponse(false, "Firm not found", error: "NotFound");

            bool duplicate = await _context.Firms.AnyAsync(f =>
                (f.FirmName.ToLower() == dto.FirmName.ToLower()
                 || f.FirmCode.ToLower() == dto.FirmCode.ToLower())
                && f.FirmId != id
                && !f.IsDeleted);

            if (duplicate)
                return ApiResponse(false, "Firm name or code already exists", error: "Duplicate");

            firm.FirmName = dto.FirmName.Trim();
            firm.FirmCode = dto.FirmCode.Trim();
            firm.IsActive = dto.IsActive;
            firm.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Firm updated successfully");
        }

        //================================
        //DELETE FIRM
        //================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFirm(int id)
        {
            var firm = await _context.Firms
                .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

            if (firm == null)
                return ApiResponse(false, "Firm not found or already deleted", error: "NotFound");

            firm.IsDeleted = true;
            firm.UpdatedAt = DateTime.Now; 

            try
            {
                await _context.SaveChangesAsync();
                return ApiResponse(true, "Firm deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error deleting firm", error: ex.Message);
            }
        }


    }
}
