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
        // GET ALL FIRMS (Firm + FirmDetails)
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
                                FirmDetailsId = fd.FirmDetailsId,
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

                return ApiResponse(true, "Firms retrieved successfully", firms);
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
            var firm = await _context.Firms
                .Where(f => f.FirmId == id && !f.IsDeleted)
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
                            FirmDetailsId = fd.FirmDetailsId,
                            Address = fd.Address,
                            ContactNumber = fd.ContactNumber,
                            ContactPerson = fd.ContactPerson,
                            GstNumber = fd.GstNumber,
                            LogoImagePath = fd.LogoImagePath,
                            IsActive = fd.IsActive
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (firm == null)
                return ApiResponse(false, "Firm not found", error: "NotFound");

            return ApiResponse(true, "Firm retrieved successfully", firm);
        }

        // ================================
        // CREATE FIRM + EMPTY FIRM DETAILS
        // ================================
        
        [HttpPost]
        public async Task<IActionResult> CreateFirm([FromBody] FirmDetailsFirmCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(false, "Invalid data");

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

                var firmDetail = new FirmDetail
                {
                    FirmId = firm.FirmId,
                    Address = dto.Address,
                    ContactNumber = dto.ContactNumber,
                    ContactPerson = dto.ContactPerson,
                    GstNumber = dto.GstNumber,
                    LogoImagePath = dto.LogoImagePath,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.FirmDetails.Add(firmDetail);
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

        // ================================
        // UPDATE FIRM
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFirm(
    int id,
    [FromBody] FirmDetailsFirmUpdateDto dto)
        {
            if (id != dto.FirmId)
                return ApiResponse(false, "FirmId mismatch");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Firm
                var firm = await _context.Firms
                    .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

                if (firm == null)
                    return ApiResponse(false, "Firm not found");

                firm.FirmName = dto.FirmName;
                firm.FirmCode = dto.FirmCode;
                firm.IsActive = dto.IsActive;
                firm.UpdatedAt = DateTime.Now;

                // FirmDetails
                var details = await _context.FirmDetails
                    .FirstOrDefaultAsync(fd => fd.FirmId == id && !fd.IsDeleted);

                if (details == null)
                    return ApiResponse(false, "Firm details not found");

                details.Address = dto.Address;
                details.ContactNumber = dto.ContactNumber;
                details.ContactPerson = dto.ContactPerson;
                details.GstNumber = dto.GstNumber;
                details.LogoImagePath = dto.LogoImagePath;
                details.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse(true, "Firm updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse(false, "Error updating firm", error: ex.Message);
            }
        }

        // ================================
        // DELETE FIRM (SOFT DELETE)
        // ================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFirm(int id)
        {
            var firm = await _context.Firms
                .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

            if (firm == null)
                return ApiResponse(false, "Firm not found", error: "NotFound");

            firm.IsDeleted = true;
            firm.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Firm deleted successfully");
        }
    }
}
