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
    // CONTROLLER: PackagePricing
    // ================================
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class PackagePricingsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public PackagePricingsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // ===============================
        // GET ALL PACKAGE PRICING
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pricings = await _context.PackagePricings
                .Include(x => x.TourPackage)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new PackagePricingResponseDto
                {
                    PricingId = x.PricingId,

                    PackageId = x.PackageId,
                    PackageName = x.TourPackage!.PackageName,

                    DayType = x.DayType,
                    PricePerPerson = x.PricePerPerson,
                    MinPersons = x.MinPersons,

                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse(
                true,
                "Package pricing fetched successfully",
                pricings);
        }

        // ===============================
        // CREATE PACKAGE PRICING
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePackagePricingDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(false, "Validation failed", ModelState);

            var package = await _context.TourPackages
                .FirstOrDefaultAsync(x =>
                    x.PackageId == dto.PackageId &&
                    !x.IsDeleted);

            if (package == null)
                return ApiResponse(false, "Tour package not found", 404);

            var pricing = new PackagePricing
            {
                PackageId = dto.PackageId,
                DayType = dto.DayType,
                PricePerPerson = dto.PricePerPerson,
                MinPersons = dto.MinPersons,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PackagePricings.Add(pricing);
            await _context.SaveChangesAsync();

            return ApiResponse(true, "Package pricing created successfully", pricing);
        }

        // ===============================
        // GET PRICING BY PACKAGE
        // ===============================
        [HttpGet("package/{packageId}")]
        public async Task<IActionResult> GetByPackage(int packageId)
        {
            var pricings = await _context.PackagePricings
                .Include(x => x.TourPackage)
                .Where(x =>
                    x.PackageId == packageId &&
                    !x.IsDeleted)
                .Select(x => new PackagePricingResponseDto
                {
                    PricingId = x.PricingId,
                    PackageId = x.PackageId,
                    PackageName = x.TourPackage!.PackageName,

                    DayType = x.DayType,
                    PricePerPerson = x.PricePerPerson,
                    MinPersons = x.MinPersons,

                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse(true, "Package pricing fetched successfully", pricings);
        }

        // ===============================
        // UPDATE PACKAGE PRICING
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePackagePricingDto dto)
        {
            var pricing = await _context.PackagePricings
                .FirstOrDefaultAsync(x =>
                    x.PricingId == id &&
                    !x.IsDeleted);

            if (pricing == null)
                return ApiResponse(false, "Package pricing not found", 404);

            if (dto.DayType != null)
                pricing.DayType = dto.DayType;

            if (dto.PricePerPerson.HasValue)
                pricing.PricePerPerson = dto.PricePerPerson.Value;

            if (dto.MinPersons.HasValue)
                pricing.MinPersons = dto.MinPersons.Value;

            pricing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Package pricing updated successfully");
        }

        // ===============================
        // DELETE PACKAGE PRICING (Soft Delete)
        // ===============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pricing = await _context.PackagePricings
                .FirstOrDefaultAsync(x =>
                    x.PricingId == id &&
                    !x.IsDeleted);

            if (pricing == null)
                return ApiResponse(false, "Package pricing not found", 404);

            pricing.IsDeleted = true;
            pricing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Package pricing deleted successfully");
        }
    }
}
