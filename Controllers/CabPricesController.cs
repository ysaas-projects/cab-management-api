using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabPricesController : BaseApiController
    {
        ApplicationDbContext _context;

        public CabPricesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =================
        // GET CABPRICES
        // =================
        [HttpGet]
        public async Task<IActionResult> CabPrices()
        {
            try
            {
                var cabPriceslst = await _context.CabPrices
                    .Where(c => c.IsDeleted.Equals(false))
                    .ToListAsync();

                if (cabPriceslst != null)
                {
                    return ApiResponse(
                        success: true,
                        message: "Data Retrieved Successfully",
                        data: cabPriceslst
                        );
                }
                else
                {
                    return ApiResponse(
                        success: false,
                        message: "Data Not Found",
                        statusCode: 404
                        );
                }
            }
            catch(Exception ex)
            {
                return ApiResponse(
                    success: false,
                    message: "Something Went Wrong",
                    error:ex.Message,
                    statusCode: 500
                    );
            }

        }


        // =====================
        // GET CABPRICE BY ID
        // =====================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabPriceById(int id)
        {
            try
            {
                var cabPrice = await _context.CabPrices
                    .FirstOrDefaultAsync(c => c.CabPriceId.Equals(id) && c.IsDeleted.Equals(false));

                if (cabPrice != null)
                {
                    return ApiResponse(
                        success: true,
                        message: "Data Retrieved Successfully",
                        data: cabPrice
                        );
                }
                else
                {
                    return ApiResponse(
                        success: false,
                        message: "Data Not Found",
                        statusCode: 404
                        );
                }
            }
            catch (Exception ex)
            {
                return ApiResponse(
                    success: false,
                    message: "Something Went Wrong",
                    error: ex.Message,
                    statusCode: 500
                    );
            }
        }

        // ====================
        // CREATE CABPRICE
        // ====================
        [HttpPost]
        public async Task<IActionResult> CreateCabPrice([FromForm]CreateCabPriceDTO dto)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    var errorslst = ModelState.Values
                        .SelectMany(e=>e.Errors
                        .Select(e=>e.ErrorMessage))
                        .ToList();

                    return ApiResponse(
                        success: false,
                        message: "Validation Error",
                        errors: errorslst,
                        statusCode: 400
                        );
                }
                else
                {
                    var cabprice = new CabPrice()
                    {
                        FirmId = dto.FirmId,
                        Price = dto.Price,
                        PriceRuleId = dto.PriceRuleId,
                        IsActive = dto.IsActive,
                        CreatedAt = dto.CreatedAt,
                    };
                    await _context.CabPrices.AddAsync(cabprice);
                    _context.SaveChanges();

                    return ApiResponse(
                        success: true,
                        message: "Data Added Successfully",
                        data: cabprice
                        );
                }
            }
            catch (Exception ex)
            {
                return ApiResponse(
                    success: false,
                    message: "Something Went Wrong",
                    error: ex.Message,
                    statusCode:500
                    );
            }
        }


        // ===================
        // UPDATE CABPRICE
        // ===================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabPrice(int id, [FromForm]UpdateCabPriceDTO dto)
        {

            try
            {
                var priceData = await _context.CabPrices
                        .FindAsync(id);
                if (priceData != null)
                {
                    if (!ModelState.IsValid && !ModelState.IsNullOrEmpty())
                    {
                        return ApiResponse(
                            success: false,
                            message: "Validation Error",
                            statusCode: 404
                            );
                    }
                    else
                    {
                        priceData.CabPriceId = dto.CabPriceId;
                        priceData.FirmId = dto.FirmId;
                        priceData.PriceRuleId = dto.PriceRuleId;
                        priceData.Price = dto.Price;
                        priceData.IsActive = dto.IsActive;
                        priceData.IsDeleted = dto.IsDeleted;
                        priceData.UpdatedAt = dto.UpdatedAt;

                        _context.SaveChanges();
                        return ApiResponse(
                            success: true,
                            message: "Record Updated Successfully"
                            );

                    }
                }
                else
                {
                    return ApiResponse(
                        success: false,
                        message: "Record Not Found",
                        statusCode: 404
                        );
                }
            }
            catch (Exception ex)
            {
                return ApiResponse(
                    success: false,
                    message: "Something Went Wrong",
                    error: ex.Message,
                    statusCode: 500
                    );
            }
        }


        // ====================
        // DELETE CABPRICE
        // ====================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabPrice(int id)
        {

            try
            {
                var existingdata = await _context.CabPrices
                     .FirstOrDefaultAsync(e => e.CabPriceId.Equals(id) && e.IsDeleted.Equals(false));

                if (existingdata != null)
                {
                    existingdata.IsDeleted = true;
                    return ApiResponse(
                        success: true,
                        message: "Record Deleted Successfully",
                        data: existingdata
                        );
                }
                else
                {
                    return ApiResponse(
                        success: false,
                        message: "Record Not Found",
                        statusCode: 404
                        );
                }
            }
            catch(Exception ex)
            {
                return ApiResponse(
                    success: false,
                    message: "Something Went Wrong",
                    error: ex.Message,
                    statusCode: 500
                    );
            }


        }

    }
}
