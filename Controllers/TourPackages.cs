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
    // CONTROLLER: TourPackages
    // ================================
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class TourPackagesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TourPackagesController> _logger;

        public TourPackagesController(
            ApplicationDbContext context,
            ILogger<TourPackagesController> logger)
        {
            _context = context;
            _logger = logger;
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
        // CREATE TOUR PACKAGE
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTourPackageDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(
                    false,
                    "Validation failed",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList());

            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var package = new TourPackage
            {
                FirmId = firmId.Value,
                PackageName = dto.PackageName,
                Description = dto.Description,
                Location = dto.Location,
                DurationDays = dto.DurationDays,
                DurationNights = dto.DurationNights,
                BasePrice = dto.BasePrice,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.TourPackages.Add(package);
            await _context.SaveChangesAsync();

            return ApiResponse(
                true,
                "Tour package created successfully",
                package);
        }

        // ===============================
        // GET ALL TOUR PACKAGES (Firm-wise)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var packages = await _context.TourPackages
                .Include(x => x.Firm)
                .Where(x =>
                    x.FirmId == firmId &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TourPackageResponseDto
                {
                    PackageId = x.PackageId,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.FirmName,

                    PackageName = x.PackageName,
                    Description = x.Description,
                    Location = x.Location,
                    DurationDays = x.DurationDays,
                    DurationNights = x.DurationNights,
                    BasePrice = x.BasePrice,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse(
                true,
                "Tour packages fetched successfully",
                packages);
        }

        // ===============================
        // GET TOUR PACKAGE BY ID
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var package = await _context.TourPackages
                .Include(x => x.Firm)
                .Where(x =>
                    x.PackageId == id &&
                    x.FirmId == firmId &&
                    !x.IsDeleted)
                .Select(x => new TourPackageResponseDto
                {
                    PackageId = x.PackageId,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.FirmName,

                    PackageName = x.PackageName,
                    Description = x.Description,
                    Location = x.Location,
                    DurationDays = x.DurationDays,
                    DurationNights = x.DurationNights,
                    BasePrice = x.BasePrice,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (package == null)
                return ApiResponse(false, "Tour package not found");

            return ApiResponse(
                true,
                "Tour package fetched successfully",
                package);
        }


        // ===============================
        // UPDATE TOUR PACKAGE
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTourPackageDto dto)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var package = await _context.TourPackages
                .FirstOrDefaultAsync(x =>
                    x.PackageId == id &&
                    x.FirmId == firmId &&
                    !x.IsDeleted);

            if (package == null)
                return ApiResponse(false, "Tour package not found", 404);

            if (dto.PackageName != null)
                package.PackageName = dto.PackageName;

            if (dto.Description != null)
                package.Description = dto.Description;

            if (dto.Location != null)
                package.Location = dto.Location;

            if (dto.DurationDays.HasValue)
                package.DurationDays = dto.DurationDays;

            if (dto.DurationNights.HasValue)
                package.DurationNights = dto.DurationNights;

            if (dto.BasePrice.HasValue)
                package.BasePrice = dto.BasePrice;

            if (dto.IsActive.HasValue)
                package.IsActive = dto.IsActive.Value;

            package.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(
                true,
                "Tour package updated successfully");
        }

        // ===============================
        // DELETE TOUR PACKAGE (Soft Delete)
        // ===============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var firmId = GetFirmIdFromToken();
            if (firmId == null)
                return ApiResponse(false, "Unauthorized", 401);

            var package = await _context.TourPackages
                .FirstOrDefaultAsync(x =>
                    x.PackageId == id &&
                    x.FirmId == firmId &&
                    !x.IsDeleted);

            if (package == null)
                return ApiResponse(false, "Tour package not found", 404);

            package.IsDeleted = true;
            package.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(
                true,
                "Tour package deleted successfully");
        }
    }
}
