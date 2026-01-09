using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabNumberDirectoryController : BaseApiController
    {
       ApplicationDbContext _context;

        public  CabNumberDirectoryController(ApplicationDbContext context)
        {
            _context= context;   
        }

        // ------------------------------
        // GET ALL CAB NUMBER DIRECTORY
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllCabNumbers()
        {
            try
            {
                var data = await _context.CabNumberDirectory
                    .Where(x => x.IsDeleted.Equals( false))
                    .ToListAsync();

                return ApiResponse(true, "CabNumber Directory fetched successfully", data);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // ------------------------------
        // GET CAB NUMBER BY ID
        // ------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabNumber(int id)
        {
            try
            {
                var cab = await _context.CabNumberDirectory
                    .FirstOrDefaultAsync(x => x.CabNumberDirectoryId == id && x.IsDeleted == false);

                if (cab == null)
                {
                    return ApiResponse(false, "CabNumber Directory not found", 400);
                }

                return ApiResponse(true, "CabNumber Directory fetched successfully", cab);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // ------------------------------
        // CREATE CAB NUMBER
        // ------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateCabNumber([FromForm] CreateCabNumberDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return ApiResponse(false, "Invalid validation", errors);
                }
                CabNumberDirectory directory = new CabNumberDirectory()
                {
                    FirmId = dto.FirmId,
                    CabId = dto.CabId,
                    CabNumber = dto.CabNumber,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                await _context.CabNumberDirectory.AddAsync(directory);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "CabNumber Directory added successfully", directory);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // ------------------------------
        // UPDATE CAB NUMBER BY ID
        // ------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabNumber(int id, [FromForm] CabNumberDirectory dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse(false, "Invalid validation", 400);
                }

                var cab = await _context.CabNumberDirectory
                    .FirstOrDefaultAsync(x => x.CabNumberDirectoryId == id && x.IsDeleted == false);

                if (cab == null)
                {
                    return ApiResponse(false, "CabNumber Directory not found", 400);
                }
                cab.CabNumberDirectoryId = dto.CabNumberDirectoryId ;  
                cab.FirmId = dto.FirmId;
                cab.CabId = dto.CabId;
                cab.CabNumber = dto.CabNumber;
                cab.IsActive = dto.IsActive;
                cab.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "CabNumber Directory updated successfully", cab);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // ------------------------------
        // DELETE CAB NUMBER  BY ID (SOFT DELETE)
        // ------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabNumber(int id)
        {
            try
            {
                CabNumberDirectory cabnumberDirectory = await _context.CabNumberDirectory
                    .FirstOrDefaultAsync(x => x.CabNumberDirectoryId == id && x.IsDeleted == false);

                if (cabnumberDirectory == null)
                {
                    return ApiResponse(false, "CabNumber Directory not found", 400);
                }

                cabnumberDirectory.IsDeleted = true;
                await _context.SaveChangesAsync();

                return ApiResponse(true, "CabNumber Directory deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
      }
   }



