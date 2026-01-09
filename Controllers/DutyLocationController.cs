using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyLocationController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public DutyLocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL DutyLocations
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetAllDutyLocations()
        {
            try
            {
                var data = await _context.DutyLocations
                    .Where(x => x.IsDeleted == false)
                    .ToListAsync();

                return ApiResponse(true, "DutyLocations retrieved successfully", data);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =====================================================
        // GET DutyLocation BY ID
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDutyLocationById(int id)
        {
            try
            {
                var data = await _context.DutyLocations
                    .FirstOrDefaultAsync(x => x.DutyLocationId == id && x.IsDeleted == false);

                if (data == null)
                    return ApiResponse(false, "Record not found", 404);

                return ApiResponse(true, "DutyLocation retrieved successfully", data);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =====================================================
        // CREATE DutyLocation
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> CreateDutyLocation([FromBody] CreateDutyLocationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Validation failed", errors);
                }

                DutyLocations dutyLocation = new DutyLocations
                {
                    FirmId = dto.FirmId,
                    DutyId = dto.DutyId,
                    Address = dto.Address,
                    GeoLocation = dto.GeoLocation,
                    CreatedAt = DateTime.Now
                };

                await _context.DutyLocations.AddAsync(dutyLocation);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "DutyLocation created successfully", dutyLocation);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =====================================================
        // UPDATE DutyLocation by Id
        // =====================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDutyLocation(int id, [FromBody] UpdateDutyLocationDTO dto)
        {
            try
            {
                var data = await _context.DutyLocations.FindAsync(id);

                if (data == null || data.IsDeleted == true)
                    return ApiResponse(false, "Record not found", 404);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Validation failed", errors);
                }

                data.FirmId = dto.FirmId;
                data.DutyId = dto.DutyId;
                data.Address = dto.Address;
                data.GeoLocation = dto.GeoLocation;
                data.IsActive = dto.IsActive;
                data.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "DutyLocation updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =====================================================
        // DELETE DutyLocation (Soft Delete)
        // =====================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDutyLocation(int id)
        {
            try
            {
                var data = await _context.DutyLocations
                    .FirstOrDefaultAsync(x => x.DutyLocationId == id && x.IsDeleted == false);

                if (data == null)
                    return ApiResponse(false, "Record not found", 400);

                data.IsDeleted = true;
                await _context.SaveChangesAsync();

                return ApiResponse(true, "DutyLocation deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
    }
}
