using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static cab_management.Models.Cab;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]

    public class CabsController : BaseApiController
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<CabsController> _logger;
        public CabsController(ApplicationDbContext context, ILogger<CabsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            if (int.TryParse(firmIdStr, out var firmId))
                return firmId;

            return null;
        }


        // GET ALL CABS
        [HttpGet]
        public async Task<IActionResult> GetAllCabs()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cabs = await _context.Cabs
                    .Include(c => c.Firm)
                    .Where(c => c.FirmId == firmId && !c.IsDeleted)
                    .Select(c => new CabResponseDto
                    {
                        CabId = c.CabId,
                        FirmId = c.FirmId,
                        FirmName = c.Firm != null ? c.Firm.FirmName : null,
                        CabType = c.CabType,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Cabs fetched successfully", cabs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCabs");
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }


        // GET PAGINATED
        [HttpGet("paginated")]
        public async Task<IActionResult> GetCabsPaginated(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                if (pageNumber < 1)
                    return ApiResponse(false, "Page number must be >= 1");

                if (pageSize < 1 || pageSize > 100)
                    return ApiResponse(false, "Page size must be 1–100");

                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var query = _context.Cabs
                    .Include(c => c.Firm)
                    .Where(c => c.FirmId == firmId && !c.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    query = query.Where(c => c.CabType.ToLower().Contains(search));
                }

                if (isActive.HasValue)
                    query = query.Where(c => c.IsActive == isActive.Value);

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(c => c.CabType)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CabResponseDto
                    {
                        CabId = c.CabId,
                        FirmId = c.FirmId,
                        FirmName = c.Firm != null ? c.Firm.FirmName : null,
                        CabType = c.CabType,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Cabs retrieved successfully", new
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
                _logger.LogError(ex, "Error in GetCabsPaginated");
                return ApiResponse(false, "Error retrieving cabs", error: ex.Message, statusCode: 500);
            }
        }



        // GET CAB BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabById(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cab = await _context.Cabs
                    .Include(c => c.Firm)
                    .Where(c => c.CabId == id && c.FirmId == firmId && !c.IsDeleted)
                    .Select(c => new CabResponseDto
                    {
                        CabId = c.CabId,
                        FirmId = c.FirmId,
                        FirmName = c.Firm != null ? c.Firm.FirmName : null,
                        CabType = c.CabType,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (cab == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                return ApiResponse(true, "Cab fetched successfully", cab);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCabById {CabId}", id);
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }

        // CREATE CAB
        [HttpPost]
        public async Task<IActionResult> CreateCab([FromBody] CreateCabDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    statusCode: 400);
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cab = new Cab
                {
                    FirmId = firmId.Value,
                    CabType = dto.CabType,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Cabs.Add(cab);
                await _context.SaveChangesAsync();

                var response = new CabResponseDto
                {
                    CabId = cab.CabId,
                    FirmId = cab.FirmId,
                    FirmName = cab.Firm != null ? cab.Firm.FirmName : null,
                    CabType = cab.CabType,
                    IsActive = cab.IsActive,
                    IsDeleted = cab.IsDeleted,
                    CreatedAt = cab.CreatedAt,
                    UpdatedAt = cab.UpdatedAt
                };

                return ApiResponse(true, "Record added successfully", response, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCab");
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }


        // UPDATE CAB
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCab(int id, [FromBody] UpdateCabDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    statusCode: 400);
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cab = await _context.Cabs
                    .Where(c => c.CabId == id && c.FirmId == firmId && !c.IsDeleted)
                    .FirstOrDefaultAsync();

                if (cab == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                if (dto.CabType != null)
                    cab.CabType = dto.CabType;

                if (dto.IsActive.HasValue)
                    cab.IsActive = dto.IsActive.Value;

                cab.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new CabResponseDto
                {
                    CabId = cab.CabId,
                    FirmId = cab.FirmId,
                    FirmName = cab.Firm != null ? cab.Firm.FirmName : null,
                    CabType = cab.CabType,
                    IsActive = cab.IsActive,
                    IsDeleted = cab.IsDeleted,
                    CreatedAt = cab.CreatedAt,
                    UpdatedAt = cab.UpdatedAt
                };

                return ApiResponse(true, "Cab updated successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCab {CabId}", id);
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }

        // DELETE (SOFT DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCab(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cab = await _context.Cabs
                    .Where(c => c.CabId == id && c.FirmId == firmId && !c.IsDeleted)
                    .FirstOrDefaultAsync();

                if (cab == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                cab.IsDeleted = true;
                cab.IsActive = false;
                cab.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Record deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCab {CabId}", id);
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }




    }
    }

