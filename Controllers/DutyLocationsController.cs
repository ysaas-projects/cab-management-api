using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DutyLocationsController : BaseApiController
    {
        ApplicationDbContext _context;
        public DutyLocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL DUTY LOCATIONS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetAllDutyLocations()
        {
            try
            {
                var dutyLocations = await _context.DutyLocations
                    .Where(d => d.IsDeleted.Equals(false))
                    .ToListAsync();

                return ApiResponse(true, "Duty locations fetched successfully", dutyLocations);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =====================================================
        // CREATE DUTY LOCATION
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> CreateDutyLocation([FromBody] AddDutyLocationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse(false, "Validation failed", ModelState);
                }

                var dutyLocation = new DutyLocation
                {
                    FirmId = dto.FirmId,
                    DutyId = dto.DutyId,
                    Address = dto.Address,
                    GeoLocation = dto.GeoLocation,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                await _context.DutyLocations.AddAsync(dutyLocation);
                await _context.SaveChangesAsync();

                return ApiResponse(
                    success: true,
                    message: "Duty Location Created Successfully",
                    data: dutyLocation,
                    statusCode: 201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }

        // =====================================================
        // GET DUTY LOCATION BY ID
        // =====================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDutyLocationById(int id)
        {
            try
            {
                var dutyLocation = await _context.DutyLocations
                    .FirstOrDefaultAsync(d => d.DutyLocationId == id && !d.IsDeleted);

                if (dutyLocation == null)
                {
                    return ApiResponse(false, "Duty Location Not Found", 404);
                }
                return ApiResponse(true, "Duty Location Fetched Successfully", dutyLocation);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }

        // =====================================================
        // UPDATE DUTY LOCATION BY ID
        // =====================================================

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDutyLocationBYId([FromBody] UpdateDutyLocationDTO dto, int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse(false, "Validation Failed", ModelState);
                }

                var dutyLocation = await _context.DutyLocations
                    .FirstOrDefaultAsync(d => d.DutyLocationId == id && !d.IsDeleted);

                if (dutyLocation == null)
                {
                    return ApiResponse(false, "Duty location not found", null);
                }

                dutyLocation.FirmId = dto.FirmId;
                dutyLocation.DutyId = dto.DutyId;
                dutyLocation.Address = dto.Address;
                dutyLocation.GeoLocation = dto.GeoLocation;
                dutyLocation.IsActive = dto.IsActive;
                dutyLocation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Duty location updated successfully", dutyLocation);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =====================================================
        // DELETE DUTY LOCATION BY ID
        // =====================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDutyLocationById(int id)
        {
            try
            {
                var dutyLocation = await _context.DutyLocations
                    .FirstOrDefaultAsync(d => d.DutyLocationId == id && !d.IsDeleted);

                if (dutyLocation == null)
                {
                    return ApiResponse(false, "Duty location not found", 400);
                }

                dutyLocation.IsDeleted = true;
                dutyLocation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Duty location deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
    }
}