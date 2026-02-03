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
    // ================================
    // CONTROLLER: Seasons
    // ================================
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class SeasonsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public SeasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // GET FirmId from JWT
        // ===============================
        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            return int.TryParse(firmIdStr, out var firmId) ? firmId : null;
        }

        // ===============================
        // CREATE SEASON
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeasonDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var season = new Season
            {
                FirmId = firmId.Value,
                SeasonName = dto.SeasonName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();

            return ApiResponse(true, "Season created successfully", season);
        }

        // ===============================
        // GET ALL SEASONS (Firm-wise)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var seasons = await _context.Seasons
                .Include(x => x.Firm)
                .Where(x =>
                    x.FirmId == firmId &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SeasonResponseDto
                {
                    SeasonId = x.SeasonId,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.FirmName,

                    SeasonName = x.SeasonName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse(true, "Seasons fetched successfully", seasons);
        }

        // ===============================
        // GET SEASON BY ID
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var season = await _context.Seasons
                .Include(x => x.Firm)
                .Where(x =>
                    x.SeasonId == id &&
                    x.FirmId == firmId &&
                    !x.IsDeleted)
                .Select(x => new SeasonResponseDto
                {
                    SeasonId = x.SeasonId,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.FirmName,

                    SeasonName = x.SeasonName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (season == null)
                return ApiResponse(false, "Season not found", 404);

            return ApiResponse(true, "Season fetched successfully", season);
        }

        // ===============================
        // UPDATE SEASON
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSeasonDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var season = await _context.Seasons
                .FirstOrDefaultAsync(x =>
                    x.SeasonId == id &&
                    x.FirmId == firmId &&
                    !x.IsDeleted);

            if (season == null)
                return ApiResponse(false, "Season not found", 404);

            if (dto.SeasonName != null)
                season.SeasonName = dto.SeasonName;

            if (dto.StartDate.HasValue)
                season.StartDate = dto.StartDate;

            if (dto.EndDate.HasValue)
                season.EndDate = dto.EndDate;

            if (dto.IsActive.HasValue)
                season.IsActive = dto.IsActive.Value;

            season.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Season updated successfully");
        }

        // ===============================
        // DELETE SEASON (Soft Delete)
        // ===============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var season = await _context.Seasons
                .FirstOrDefaultAsync(x =>
                    x.SeasonId == id &&
                    x.FirmId == firmId &&
                    !x.IsDeleted);

            if (season == null)
                return ApiResponse(false, "Season not found", 404);

            season.IsDeleted = true;
            season.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Season deleted successfully");
        }
    }
}
