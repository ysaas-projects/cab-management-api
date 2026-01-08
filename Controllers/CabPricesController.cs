using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabPricesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CabPricesController> _logger;
        public CabPricesController(ApplicationDbContext context, ILogger<CabPricesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        //=========================================
        //  GET ALL CAB PRICES
        //=========================================
        [HttpGet]
        public async Task<IActionResult> GetCabPrices()
        {
            try
            {
                var prices = await _context.CabPrices.Where(e => !e.IsDeleted).
                    Select(c => new CabPriceResponseDto
                    {
                        CabPriceId = c.CabPriceId,
                        FirmId = c.FirmId,
                        CabId = c.CabId,
                        PriceRuleId = c.PriceRuleId,
                        Price = c.Price,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        IsDeleted=c.IsDeleted
                    }).ToListAsync();
                return ApiResponse(true, "cab prices retrieved successfully", prices);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cab prices");
                return ApiResponse(false, "Error retrieving cab prices", error: ex.Message);
            }
        }
        
        //=========================================
        //  GET BY ID CABPRICES
        //=========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabPrice(int id)
        {
            try
            {
                var prices = await _context.CabPrices.Where(e => e.CabPriceId == id && !e.IsDeleted).
                    Select(c => new CabPriceResponseDto
                    {
                        CabPriceId = c.CabPriceId,
                        FirmId = c.FirmId,
                        CabId=c.CabId,
                        PriceRuleId = c.PriceRuleId,
                        Price = c.Price,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        IsDeleted = c.IsDeleted
                    }).FirstOrDefaultAsync();
                if(prices==null)
                
                    return ApiResponse(false, "Cab Price not found", error: "Not found");
                    return ApiResponse(true, "cab price retrieved successfully", prices);
               
            }
            catch(Exception ex)
            {
                return ApiResponse(false, "Error retrieving cab price", error: ex.Message);
            }
        }
       
        //=========================================
        //  CREATE CABPRICES
        //=========================================
        [HttpPost]
        public async Task<IActionResult> CreateCabPrice([FromBody] CreateCabPriceDto dto)
        {
            if(!ModelState.IsValid)
            {
                return ApiResponse(false, "validation failed", errors: ModelState.Values.
                    SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList());
            }
            try
            {
                var cabprice = new CabPrice
                {
                    FirmId = dto.FirmId,
                    CabId = dto.CabId,
                    PriceRuleId = dto.PriceRuleId,
                    Price = dto.Price,
                    IsActive = dto.IsActive,
                    CreatedAt = dto.CreatedAt,
                    IsDeleted = false
                };
                _context.CabPrices.Add(cabprice);
                await _context.SaveChangesAsync();
                return ApiResponse(true, "Cab price created successfully", cabprice, statusCode: 201);
            }
            catch(Exception ex)
            {
                return ApiResponse(false, "Error creating cab price", error: ex.Message);
            }
        }
       
        //=========================================
        // UPDATE CABPRICES
        //=========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabPrice(int id, [FromBody] UpdateCabPriceDto dto)
        {
            try
            {
                var cabprice = await _context.CabPrices.Where(c => c.CabPriceId == id && !c.IsDeleted)
                    .FirstOrDefaultAsync();
                if(cabprice==null)
                
                    return ApiResponse(false, "cab price not found", error: "not found");
                    cabprice.FirmId = dto.FirmId;
                    cabprice.CabId = dto.CabId ?? cabprice.CabId;
                    cabprice.PriceRuleId = dto.PriceRuleId ?? cabprice.PriceRuleId;
                    cabprice.Price = dto.Price ?? cabprice.Price;
                    cabprice.IsActive = dto.IsActive ?? cabprice.IsActive;
                    cabprice.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    return ApiResponse(true, "cab price updated successfully");


                
                
            }
            catch(Exception ex)
            {
                return ApiResponse(false, "Error updating cabprice", error: ex.Message);
            }

        }
       
        //=========================================
        //  DELETE CABPRICES
        //=========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabPrice(int id)
        {
            try
            {
                var cabprice = await _context.CabPrices.Where(c => c.CabPriceId == id && !c.IsDeleted)
                    .FirstOrDefaultAsync();
                if(cabprice==null)
                
                    return ApiResponse(false, "cab price not found", error: "not found");
                    cabprice.IsDeleted = true;
                    cabprice.IsActive = false;
                    cabprice.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return ApiResponse(true, "cab price deleted successfully");
                
                
            }
            catch(Exception ex)
            {
                return ApiResponse(false, "Error deleting cab price", error: ex.Message);
            }
        }



    }
}
