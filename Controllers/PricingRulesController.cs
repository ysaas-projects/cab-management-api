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
        JwtBearerDefaults.AuthenticationScheme + "," +
        CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class PricingRulesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PricingRulesController> _logger;

        public PricingRulesController(
            ApplicationDbContext context,
            ILogger<PricingRulesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ================================
        // GET FirmId from Token
        // ================================
        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            return int.TryParse(firmIdStr, out var firmId)
                ? firmId
                : null;
        }

        // ================================
        // GET ALL PRICING RULES (BY FIRM)
        // ================================
        [HttpGet]
        public async Task<IActionResult> GetPricingRules()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized();

                var rules = await _context.PricingRules
                 .Include(r => r.Firm) 
                    .Where(r =>
                        r.FirmId == firmId &&
                        !r.IsDeleted)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new PricingRuleResponseDto
                    {
                        PricingRuleId = r.PricingRuleId,
                        FirmId = r.FirmId,
                        FirmName = r.Firm.FirmName, // ✅ DISPLAY
                        RuleDetails = r.RuleDetails,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(
                    true,
                    "Pricing rules retrieved successfully",
                    rules
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricing rules");
                return ApiResponse(false, "Error retrieving pricing rules", error: ex.Message);
            }
        }

        // ================================
        // GET PRICING RULE BY ID (BY FIRM)
        // ================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPricingRule(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized();

                var rule = await _context.PricingRules
                                     .Include(r => r.Firm)

                    .Where(r =>
                        r.PricingRuleId == id &&
                        r.FirmId == firmId &&
                        !r.IsDeleted)
                    .Select(r => new PricingRuleResponseDto
                    {
                        PricingRuleId = r.PricingRuleId,
                        FirmId = r.FirmId,
                        FirmName = r.Firm.FirmName, // ✅ DISPLAY

                        RuleDetails = r.RuleDetails,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (rule == null)
                    return ApiResponse(false, "Pricing rule not found", error: "Not Found");

                return ApiResponse(true, "Pricing rule retrieved successfully", rule);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving pricing rule", error: ex.Message);
            }
        }

        // ================================
        // CREATE PRICING RULE (BY FIRM)
        // ================================
        [HttpPost]
        public async Task<IActionResult> CreatePricingRule(
            [FromBody] CreatePricingRuleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(
                    false,
                    "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                );
            }

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized();

                var rule = new PricingRule
                {
                    FirmId = firmId.Value,
                    RuleDetails = dto.RuleDetails,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.PricingRules.Add(rule);
                await _context.SaveChangesAsync();

                return ApiResponse(
                    true,
                    "Pricing rule created successfully",
                    rule,
                    statusCode: 201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error creating pricing rule", error: ex.Message);
            }
        }

        // ================================
        // UPDATE PRICING RULE (BY FIRM)
        // ================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePricingRule(
            int id,
            [FromBody] UpdatePricingRuleDto dto)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized();

                var rule = await _context.PricingRules
                    .Where(r =>
                        r.PricingRuleId == id &&
                        r.FirmId == firmId &&
                        !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (rule == null)
                    return ApiResponse(false, "Pricing rule not found", error: "Not Found");

                rule.RuleDetails = dto.RuleDetails ?? rule.RuleDetails;
                rule.IsActive = dto.IsActive ?? rule.IsActive;
                rule.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Pricing rule updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error updating pricing rule", error: ex.Message);
            }
        }

        // ================================
        // DELETE PRICING RULE (SOFT DELETE)
        // ================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingRule(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized();

                var rule = await _context.PricingRules
                    .Where(r =>
                        r.PricingRuleId == id &&
                        r.FirmId == firmId &&
                        !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (rule == null)
                    return ApiResponse(false, "Pricing rule not found", error: "Not Found");

                rule.IsDeleted = true;
                rule.IsActive = false;
                rule.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Pricing rule deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error deleting pricing rule", error: ex.Message);
            }
        }
    }
}
