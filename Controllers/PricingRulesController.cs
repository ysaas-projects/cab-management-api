using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
   
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

        // 🔹 GET ALL PRIZING RULES
        [HttpGet]
        public async Task<IActionResult> GetPricingRules()
        {
            try
            {
                var rules = await _context.PricingRules
                    .Where(r => !r.IsDeleted)
                    .Select(r => new PricingRuleResponseDto
                    {
                        PricingRuleId = r.PricingRuleId,
                        FirmId = r.FirmId,
                        RoleDetails = r.RoleDetails,
                        IsActive = r.IsActive,
                        IsDeleted = r.IsDeleted,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Pricing rules retrieved successfully", rules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pricing rules");
                return ApiResponse(false, "Error retrieving pricing rules", error: ex.Message);
            }
        }

        // 🔹 GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPricingRule(int id)
        {
            try
            {
                var rule = await _context.PricingRules
                    .Where(r => r.PricingRuleId == id && !r.IsDeleted)
                    .Select(r => new PricingRuleResponseDto
                    {
                        PricingRuleId = r.PricingRuleId,
                        FirmId = r.FirmId,
                        RoleDetails = r.RoleDetails,
                        IsActive = r.IsActive,
                        IsDeleted = r.IsDeleted,
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

        // 🔹 CREATE
        [HttpPost]
        public async Task<IActionResult> CreatePricingRule([FromBody] CreatePricingRuleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList());
            }

            try
            {
                var rule = new PricingRule
                {
                    FirmId = dto.FirmId,              // ✅ FirmId comes from request
                    RoleDetails = dto.RoleDetails,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.PricingRules.Add(rule);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Pricing rule created successfully", rule, statusCode: 201);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error creating pricing rule", error: ex.Message);
            }
        }

        // 🔹 UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePricingRule(int id, [FromBody] UpdatePricingRuleDto dto)
        {
            try
            {
                var rule = await _context.PricingRules
                    .Where(r => r.PricingRuleId == id && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (rule == null)
                    return ApiResponse(false, "Pricing rule not found", error: "Not Found");
               
                rule.FirmId = dto.FirmId;

                rule.RoleDetails = dto.RoleDetails ?? rule.RoleDetails;
                rule.IsActive = dto.IsActive ?? rule.IsActive;
                rule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Pricing rule updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error updating pricing rule", error: ex.Message);
            }
        }

        // 🔹 DELETE (SOFT DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingRule(int id)
        {
            try
            {
                var rule = await _context.PricingRules
                    .Where(r => r.PricingRuleId == id && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (rule == null)
                    return ApiResponse(false, "Pricing rule not found", error: "Not Found");

                rule.IsDeleted = true;
                rule.IsActive = false;
                rule.UpdatedAt = DateTime.UtcNow;

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
