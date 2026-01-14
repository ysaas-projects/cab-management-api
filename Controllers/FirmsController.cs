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
    //[Authorize(AuthenticationSchemes =
    //    JwtBearerDefaults.AuthenticationScheme + "," +
    //    CookieAuthenticationDefaults.AuthenticationScheme)]
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

                        FirmDetails = f.FirmDetails != null && !f.FirmDetails.IsDeleted
                            ? new FirmDetailsDto
                            {
                                FirmDetailsId = f.FirmDetails.FirmDetailsId,
                                Address = f.FirmDetails.Address,
                                ContactNumber = f.FirmDetails.ContactNumber,
                                ContactPerson = f.FirmDetails.ContactPerson,
                                GstNumber = f.FirmDetails.GstNumber,
                                LogoImagePath = f.FirmDetails.LogoImagePath,
                                IsActive = f.FirmDetails.IsActive
                            }
                            : null
                    })
                    .ToListAsync();

                return ApiResponse(true, "Firm retrieved successfully", firms);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving firms", error: ex.Message);
            }
        }

        // GET PAGINATED FIRMS
        [HttpGet("paginated")]
        public async Task<IActionResult> GetFirmsPaginated(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = "",
            [FromQuery] bool? isActive = null)
        {
            try
            {
                if (pageNumber < 1)
                    return ApiResponse(false, "Page number must be >= 1");

                if (pageSize < 1 || pageSize > 100)
                    return ApiResponse(false, "Page size must be 1–100");

                var query = _context.Firms
                    .Where(f => !f.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim().ToLower();
                    query = query.Where(f => f.FirmName.ToLower().Contains(s) || f.FirmCode.ToLower().Contains(s));
                }

                if (isActive.HasValue)
                    query = query.Where(f => f.IsActive == isActive.Value);

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(f => f.FirmName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => new FirmResponseDto
                    {
                        FirmId = f.FirmId,
                        FirmName = f.FirmName,
                        FirmCode = f.FirmCode,
                        IsActive = f.IsActive,
                        FirmDetails = f.FirmDetails != null && !f.FirmDetails.IsDeleted
                            ? new FirmDetailsDto
                            {
                                FirmDetailsId=f.FirmDetails.FirmDetailsId,
                                Address = f.FirmDetails.Address,
                                ContactNumber = f.FirmDetails.ContactNumber,
                                ContactPerson = f.FirmDetails.ContactPerson,
                                GstNumber = f.FirmDetails.GstNumber,
                                LogoImagePath = f.FirmDetails.LogoImagePath,
                                IsActive = f.FirmDetails.IsActive
                            }
                            : null
                    })
                    .ToListAsync();

                return ApiResponse(true, "Firms retrieved successfully", new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Items = items
                });
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
                        FirmDetails = f.FirmDetails != null && !f.FirmDetails.IsDeleted
                            ? new
                            {
                                FirmDetailsId=f.FirmDetails.FirmDetailsId,
                                f.FirmDetails.Address,
                                f.FirmDetails.ContactNumber,
                                f.FirmDetails.ContactPerson,
                                f.FirmDetails.GstNumber,
                                f.FirmDetails.LogoImagePath,
                                f.FirmDetails.IsActive
                            }
                            : null
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
