using System.Security.Claims;
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
    CookieAuthenticationDefaults.AuthenticationScheme + "," +
    JwtBearerDefaults.AuthenticationScheme)]
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

        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            if (int.TryParse(firmIdStr, out var firmId))
                return firmId;

            return null;
        }


        [HttpGet]
        public async Task<IActionResult> GetCabPrices()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var prices = await _context.CabPrices
                    .Include(cp => cp.Firm)
                    .Include(cp => cp.Cab)
                    .Include(cp => cp.PricingRule)
                    .Where(cp => cp.FirmId == firmId && !cp.IsDeleted)
                    .Select(cp => new CabPriceResponseDto
                    {
                        CabPriceId = cp.CabPriceId,
                        FirmId = cp.FirmId,
                        FirmName = cp.Firm.FirmName,
                        CabId = cp.CabId,
                        CabType = cp.Cab.CabType,
                        PricingRuleId = cp.PricingRuleId,
                        PricingRuleName = cp.PricingRule.RuleDetails,
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
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabPrice(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var price = await _context.CabPrices
                    .Include(cp => cp.Firm)
                    .Include(cp => cp.Cab)
                    .Include(cp => cp.PricingRule)
                    .Where(cp =>
                        cp.CabPriceId == id &&
                        cp.FirmId == firmId &&
                        !cp.IsDeleted)
                    .Select(cp => new CabPriceResponseDto
                    {
                        CabPriceId = cp.CabPriceId,
                        FirmId = cp.FirmId,
                        FirmName = cp.Firm.FirmName,
                        CabId = cp.CabId,
                        CabType = cp.Cab.CabType,
                        PricingRuleId = cp.PricingRuleId,
                        PricingRuleName = cp.PricingRule.RuleDetails,
                        Price = cp.Price,
                        IsActive = cp.IsActive,
                        CreatedAt = cp.CreatedAt,
                        UpdatedAt = cp.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (price == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                return ApiResponse(true, "Cab price fetched successfully", price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cab price {CabPriceId}", id);
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
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
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList(),
                    statusCode: 400);
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cabPrice = new CabPrice
                {
                    FirmId = firmId.Value, // 🔥 FROM TOKEN
                    CabId = dto.CabId,
                    PricingRuleId = dto.PricingRuleId,
                    Price = dto.Price,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CabPrices.Add(cabPrice);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Record added successfully", new
                {
                    cabPrice.CabPriceId
                }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCabPrice");
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
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
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cabPrice = await _context.CabPrices
                    .Where(cp =>
                        cp.CabPriceId == id &&
                        cp.FirmId == firmId &&
                        !cp.IsDeleted)
                    .FirstOrDefaultAsync();

                if (cabPrice == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                if (dto.CabId.HasValue)
                    cabPrice.CabId = dto.CabId.Value;

                if (dto.PricingRuleId.HasValue)
                    cabPrice.PricingRuleId = dto.PricingRuleId.Value;

                if (dto.Price.HasValue)
                    cabPrice.Price = dto.Price.Value;

                if (dto.IsActive.HasValue)
                    cabPrice.IsActive = dto.IsActive.Value;

                cabPrice.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Cab price updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCabPrice {CabPriceId}", id);
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
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
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

                var cabPrice = await _context.CabPrices
                    .Where(cp =>
                        cp.CabPriceId == id &&
                        cp.FirmId == firmId &&
                        !cp.IsDeleted)
                    .FirstOrDefaultAsync();

                if (cabPrice == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                cabPrice.IsDeleted = true;
                cabPrice.IsActive = false;
                cabPrice.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Record deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCabPrice {CabPriceId}", id);
                return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
            }
        }

		// =====================================================
		// GET CAB × PRICING RULE MATRIX
		// =====================================================
		[HttpGet("matrix")]
		public async Task<IActionResult> GetCabPricingMatrix()
		{
			try
			{
				var firmId = GetFirmIdFromToken();
				if (firmId == null)
					return ApiResponse(false, "Invalid firm access!", error: "Unauthorized", statusCode: 401);

				// Load base data
				var cabs = await _context.Cabs
					.Where(c =>
						c.FirmId == firmId &&
						!c.IsDeleted &&
						c.IsActive)
					.Select(c => new
					{
						c.CabId,
						c.CabType
					})
					.ToListAsync();

				var pricingRules = await _context.PricingRules
					.Where(pr =>
						pr.FirmId == firmId &&
						!pr.IsDeleted &&
						pr.IsActive)
					.Select(pr => new
					{
						pr.PricingRuleId,
						pr.RuleDetails
					})
					.ToListAsync();

				var cabPrices = await _context.CabPrices
					.Where(cp =>
						cp.FirmId == firmId &&
						!cp.IsDeleted)
					.Select(cp => new
					{
						cp.CabPriceId,
						cp.CabId,
						cp.PricingRuleId,
						cp.Price
					})
					.ToListAsync();

				// CROSS JOIN + LEFT JOIN (LINQ)
				var result =
					from cab in cabs
					from rule in pricingRules
					join price in cabPrices
						on new { cab.CabId, rule.PricingRuleId }
						equals new { price.CabId, price.PricingRuleId }
						into priceGroup
					from pg in priceGroup.DefaultIfEmpty()
					select new CabPricingMatrixDto
					{
						FirmId = firmId.Value,
						CabId = cab.CabId,
						CabType = cab.CabType,
						PricingRuleId = rule.PricingRuleId,
						PricingRuleName = rule.RuleDetails,
						CabPriceId = pg?.CabPriceId,
						Price = pg?.Price   // 👈 NULL if no match
					};

				return ApiResponse(true, "Cab pricing matrix fetched successfully", result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching cab pricing matrix");
				return ApiResponse(false, "Something went wrong", error: ex.Message, statusCode: 500);
			}
		}



	}
}
