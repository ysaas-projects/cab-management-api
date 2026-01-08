using cab_management.Data;
using DriverDetails.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverDetailsController :BaseApiController
    {
        ApplicationDbContext _context;
        public DriverDetailsController(ApplicationDbContext context)
        {
           _context = context;
        }

        // ------------------------------
        // GET ALL DRIVER DETAILS
        // ------------------------------

        [HttpGet]
         public async Task<IActionResult> GetDriverDetails()
        {
            try
            {
                var DriverDetails = await _context.DriverDetails
                .Where(c => c.IsDeleted != true)
                .ToListAsync();

                return ApiResponse(true, "DriverDetails Fetched Successfully", DriverDetails);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Errror", ex.Message);
            }
        }

        // ------------------------------
        //GET DRIVER DETAILS BY ID
        // ------------------------------

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDriverDetailsByID(int id)
        {
            try
            {
                 var driverDetail= _context.DriverDetails.FirstOrDefault(d=>d.DriverDetailId.Equals(id) && d.IsDeleted.Equals(false));
                if (driverDetail != null)
                {
                 return ApiResponse(true, "DriverDetail Fetched Successfully", driverDetail);
                }
                else
                {
                 return ApiResponse(false, "DriverDetail Not Found",404);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Errror", ex.Message);
            }
        }

        // ------------------------------
        // CREATE DRIVER DETAILS
        // ------------------------------

        [HttpPost]
         public async Task<IActionResult> CreateDriverDetails([FromBody] AddDriverDetailDTO  dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Validation Failed", errors);
                }

                DriverDetail driverDetail = new DriverDetail()
                {
                  FirmId= dto.FirmId,
                  UserId= dto.UserId,
                  DriverName= dto.DriverName,
                  MobileNumber = dto.MobileNumber,
                  IsActive= dto.IsActive,
                  CreatedAt =DateTime.Now,
                  IsDeleted=false
                };
                await _context.DriverDetails.AddAsync(driverDetail);
                await _context.SaveChangesAsync();
                return ApiResponse(false, "DriverDetails Added Successfully", driverDetail);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }

        // ------------------------------
        // UPDATE DRIVER DETAILS
        // ------------------------------

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDriverDetails(int id,[FromBody] UpdateDriverDetailDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorlst = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Validation Failed");
                }

                var driverDetail = _context.DriverDetails.FirstOrDefault(d => d.DriverDetailId.Equals(id) && d.IsDeleted.Equals(false));
                if (driverDetail != null);

                    if (driverDetail == null)
                {
                    return ApiResponse(false, "Record Not Found", null);
                }
                driverDetail.UserId = dto.UserId;
                driverDetail.FirmId = dto.FirmId;
                driverDetail.MobileNumber = dto.MoblieNumber;
                driverDetail.DriverName = dto.DriverName;
                driverDetail.IsActive = dto.IsActive;
                driverDetail.IsDeleted = dto.IsDeleted;
                driverDetail.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(
                    success: true,
                    message: "DriverDetails Updated Successfully",
                    data: driverDetail
                );
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }

        // ------------------------------
        // DELETE DRIVER DETAILS
        // ------------------------------

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var driverDetails = await _context.DriverDetails
                .FirstOrDefaultAsync(d => d.DriverDetailId.Equals(id) && d.IsDeleted.Equals(false));

                if (driverDetails == null)
                {
                    return ApiResponse(
                        success: false,
                        message: "Record Not Found",
                        statusCode: 400
                        );
                }
                else
                {
                    driverDetails.IsDeleted = true;
                    _context.SaveChanges();

                }
                return ApiResponse(
                    success: true,
                    message: "DriverDetails Deleted Successfully"
                    );
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
    }
}
