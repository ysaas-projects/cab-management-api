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
    public class CabNumberDirectoryController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CabNumberDirectoryController> _logger;

        public CabNumberDirectoryController(
            ApplicationDbContext context,
            ILogger<CabNumberDirectoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ===============================
        // GET FirmId FROM TOKEN
        // ===============================
        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            if (int.TryParse(firmIdStr, out var firmId))
                return firmId;

            return null;
        }

        // ===============================
        // GET ALL CAB NUMBERS
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var data = await _context.CabNumberDirectory
                    .Include(x => x.Cab)
                    .Include(x => x.Firm)
                    .Where(x => x.FirmId == firmId && !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new CabNumberDirectoryResponseDto
                    {
                        CabNumberDirectoryId = x.CabNumberDirectoryId,
                        FirmId = x.FirmId,
                        FirmName = x.Firm.FirmName,
                        CabId = x.CabId,
                        CabType = x.Cab.CabType,
                        CabNumber = x.CabNumber,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Cab numbers fetched successfully", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll CabNumberDirectory");
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // ===============================
        // GET PAGINATED
        // ===============================
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginated(
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
                    return ApiResponse(false, "Page size must be between 1 and 100");

                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var query = _context.CabNumberDirectory
                    .Include(x => x.Cab)
                    .Include(x => x.Firm)
                    .Where(x => x.FirmId == firmId && !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    query = query.Where(x => x.CabNumber.ToLower().Contains(search));
                }

                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new CabNumberDirectoryResponseDto
                    {
                        CabNumberDirectoryId = x.CabNumberDirectoryId,
                        FirmId = x.FirmId,
                        FirmName = x.Firm.FirmName,
                        CabId = x.CabId,
                        CabType = x.Cab.CabType,
                        CabNumber = x.CabNumber,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Cab numbers retrieved successfully", new
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
                _logger.LogError(ex, "Error in GetPaginated CabNumberDirectory");
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // ===============================
        // GET BY ID
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var record = await _context.CabNumberDirectory
                    .Include(x => x.Cab)
                    .Include(x => x.Firm)
                    .Where(x => x.CabNumberDirectoryId == id &&
                                x.FirmId == firmId &&
                                !x.IsDeleted)
                    .Select(x => new CabNumberDirectoryResponseDto
                    {
                        CabNumberDirectoryId = x.CabNumberDirectoryId,
                        FirmId = x.FirmId,
                        FirmName = x.Firm.FirmName,
                        CabId = x.CabId,
                        CabType = x.Cab.CabType,
                        CabNumber = x.CabNumber,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (record == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                return ApiResponse(true, "Cab number fetched successfully", record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById CabNumberDirectory {Id}", id);
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // ===============================
        // CREATE
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCabNumberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList(),
                    statusCode: 400);
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var exists = await _context.CabNumberDirectory.AnyAsync(x =>
                    x.FirmId == firmId &&
                    x.CabNumber == dto.CabNumber &&
                    !x.IsDeleted);

                if (exists)
                    return ApiResponse(false, "Cab number already exists", statusCode: 409);

                var entity = new CabNumberDirectory
                {
                    FirmId = firmId.Value,
                    CabId = dto.CabId,
                    CabNumber = dto.CabNumber,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CabNumberDirectory.Add(entity);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab number added successfully", new
                {
                    entity.CabNumberDirectoryId
                }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create CabNumberDirectory");
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // ===============================
        // UPDATE
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCabNumberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList(),
                    statusCode: 400);
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                if (id != dto.CabNumberDirectoryId)
                    return ApiResponse(false, "ID mismatch", statusCode: 400);

                var entity = await _context.CabNumberDirectory
                    .Where(x => x.CabNumberDirectoryId == id &&
                                x.FirmId == firmId &&
                                !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (entity == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                entity.CabId = dto.CabId;
                entity.CabNumber = dto.CabNumber;
                entity.IsActive = dto.IsActive;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab number updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update CabNumberDirectory {Id}", id);
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }

        // ===============================
        // DELETE (SOFT)
        // ===============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var entity = await _context.CabNumberDirectory
                    .Where(x => x.CabNumberDirectoryId == id &&
                                x.FirmId == firmId &&
                                !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (entity == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                entity.IsDeleted = true;
                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab number deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete CabNumberDirectory {Id}", id);
                return ApiResponse(false, "Something went wrong",
                    error: ex.Message, statusCode: 500);
            }
        }
        // ==========================================
        // GET: Cab-wise Cab Numbers (JOIN)
        // ==========================================
        [HttpGet("by-cab")]
        public async Task<IActionResult> GetCabWiseCabNumbers()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!",
                        error: "Unauthorized", statusCode: 401);

                var data = await _context.Cabs
                    .Where(c =>
                        c.FirmId == firmId &&
                        !c.IsDeleted)
                    .Select(c => new CabWithNumbersDto
                    {
                        CabId = c.CabId,
                        CabType = c.CabType,

                        CabNumbers = _context.CabNumberDirectory
                            .Where(n =>
                                n.CabId == c.CabId &&
                                !n.IsDeleted)
                            .Select(n => new CabNumberOnlyDto
                            {
                                CabNumberDirectoryId = n.CabNumberDirectoryId,
                                CabNumber = n.CabNumber,
                                IsActive = n.IsActive
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Cab-wise cab numbers fetched successfully",
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong",
                    error = ex.Message
                });
            }
        }

    }
}
