using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabPricesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CabPricesController> _logger;

        public CabPricesController(
            ApplicationDbContext context,
            ILogger<CabPricesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================================================
        // GET ALL CAB PRICES (FirmName + CabType)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetCabPrices()
        {
            try
            {
                var prices = await _context.CabPrices
                    .Where(cp => !cp.IsDeleted)
                    .Include(cp => cp.Firm)
                    .Include(cp => cp.Cab)
                    .Select(cp => new CabPriceResponseDto
                    {
                        CabPriceId = cp.CabPriceId,

                        FirmId = cp.FirmId,
                        FirmName = cp.Firm.FirmName,

                        CabId = cp.CabId,
                        CabType = cp.Cab.CabType,

                        PricingRuleId = cp.PricingRuleId,
                        Price = cp.Price,

                        IsActive = cp.IsActive,
                        CreatedAt = cp.CreatedAt,
                        UpdatedAt = cp.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Cab prices fetched successfully", prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cab prices");
                return ApiResponse(false, "Error fetching cab prices", error: ex.Message);
            }
        }

        // =====================================================
        // GET CAB PRICE BY ID
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabPrice(int id)
        {
            try
            {
                var price = await _context.CabPrices
                    .Where(cp => cp.CabPriceId == id && !cp.IsDeleted)
                    .Include(cp => cp.Firm)
                    .Include(cp => cp.Cab)
                    .Select(cp => new CabPriceResponseDto
                    {
                        CabPriceId = cp.CabPriceId,

                        FirmId = cp.FirmId,
                        FirmName = cp.Firm.FirmName,

                        CabId = cp.CabId,
                        CabType = cp.Cab.CabType,

                        PricingRuleId = cp.PricingRuleId,
                        Price = cp.Price,

                        IsActive = cp.IsActive,
                        CreatedAt = cp.CreatedAt,
                        UpdatedAt = cp.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (price == null)
                    return ApiResponse(false, "Cab price not found", error: "NotFound");

                return ApiResponse(true, "Cab price retrieved successfully", price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cab price");
                return ApiResponse(false, "Error retrieving cab price", error: ex.Message);
            }
        }

        // =====================================================
        // CREATE CAB PRICE
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> CreateCabPrice([FromBody] CreateCabPriceDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ApiResponse(false, "Validation failed", errors: errors);
            }

            try
            {
                var cabPrice = new CabPrice
                {
                    FirmId = dto.FirmId,
                    CabId = dto.CabId,
                    PricingRuleId = dto.PricingRuleId,
                    Price = dto.Price,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.CabPrices.Add(cabPrice);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab price created successfully", new
                {
                    cabPrice.CabPriceId
                }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cab price");
                return ApiResponse(false, "Error creating cab price", error: ex.Message);
            }
        }

        // =====================================================
        // UPDATE CAB PRICE
        // =====================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabPrice(int id, [FromBody] UpdateCabPriceDto dto)
        {
            try
            {
                var cabPrice = await _context.CabPrices
                    .FirstOrDefaultAsync(cp => cp.CabPriceId == id && !cp.IsDeleted);

                if (cabPrice == null)
                    return ApiResponse(false, "Cab price not found", error: "NotFound");

                cabPrice.FirmId = dto.FirmId;
                cabPrice.CabId = dto.CabId ?? cabPrice.CabId;
                cabPrice.PricingRuleId = dto.PricingRuleId ?? cabPrice.PricingRuleId;
                cabPrice.Price = dto.Price ?? cabPrice.Price;
                cabPrice.IsActive = dto.IsActive ?? cabPrice.IsActive;
                cabPrice.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab price updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cab price");
                return ApiResponse(false, "Error updating cab price", error: ex.Message);
            }
        }

        // =====================================================
        // DELETE CAB PRICE (SOFT DELETE)
        // =====================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabPrice(int id)
        {
            try
            {
                var cabPrice = await _context.CabPrices
                    .FirstOrDefaultAsync(cp => cp.CabPriceId == id && !cp.IsDeleted);

                if (cabPrice == null)
                    return ApiResponse(false, "Cab price not found", error: "NotFound");

                cabPrice.IsDeleted = true;
                cabPrice.IsActive = false;
                cabPrice.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab price deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cab price");
                return ApiResponse(false, "Error deleting cab price", error: ex.Message);
            }
        }
    }
}
