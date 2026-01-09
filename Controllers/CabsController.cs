using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static cab_management.Models.Cab;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CabsController : BaseApiController
    {

        ApplicationDbContext _context;
        public CabsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ====================
        // GET ALL CABS
        // ====================
        [HttpGet]
        public async Task<IActionResult> GetAllCabs()
        {
            try
            {

                var cabs = await _context.Cabs
                .Where(c => c.IsDeleted != true)
                .ToListAsync();

                return ApiResponse(
                    success: true,
                    message: "Cabs Fetched Successfully",
                    data: cabs
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

        // ====================
        // GET CAB BY ID
        // ====================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabById(int id)
        {
                try
                {
                    var cab = _context.Cabs.
                    FirstOrDefault(x => x.CabId.Equals(id) && x.IsDeleted.Equals(false));

                    if (cab != null)
                    {
                        return ApiResponse(
                            success:true,
                            message:"Cab Fetched Successfully",
                            data:cab
                            );
                    }
                    else
                    {
                        return ApiResponse(
                            success:false,
                            message:"Record Not Found",
                            statusCode:404
                            );
                    }

                }
                catch (Exception ex)
                {
                    return ApiResponse(
                        success:false,
                        message:"Something Went Wrong",
                        error:ex.Message,
                        statusCode:500
                        );
                }
        }

            // ===================
            // CREATE CAB
            // ===================
            [HttpPost]
            public async Task<IActionResult> CreateCab([FromForm] CreateCabDto dto)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var errorlst = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(x => x.ErrorMessage)
                            .ToList();

                        return ApiResponse(
                            success:false,
                            message:"Validation Failed",
                            errors:errorlst,
                            statusCode:400
                            );
                    }

                    Cab cab = new Cab()
                    {
                        OrganizationId = dto.OrganizationId,
                        CabType = dto.CabType,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        IsActive = dto.IsActive
                    };

                    await _context.Cabs.AddAsync(cab);
                    _context.SaveChanges();

                    return ApiResponse(
                    success: true,
                    message: "Record Added Successfully",
                    data:cab
                    );
                }
                catch (Exception ex)
                {
                    return ApiResponse(
                        success:false,
                        message:"Something Went Wrong",
                        error:ex.Message,
                        statusCode:500
                        );
                }
            }
            
            // =====================
            // UPDATE CAB
            // =====================
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateCab(int id, [FromForm]UpdateCabDto dto)
            {

                try
                {
                    var cab = await _context.Cabs.
                    FindAsync(id);
                    if(cab!=null)
                    {
                        if (!ModelState.IsValid && !ModelState.IsNullOrEmpty())
                        {
                            var errorlst = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(x => x.ErrorMessage)
                                .ToList();
                            return ApiResponse(
                                success: false,
                                message: "Validation Failed",
                                errors: errorlst,
                                statusCode: 400
                                );
                        }
                        else
                        {
                            cab.OrganizationId = dto.OrganizationId;
                            cab.CabType = dto.CabType;
                            cab.UpdetedAt = DateTime.Now;
                            cab.IsDeleted = dto.IsDeleted;
                            cab.IsActive = dto.IsActive;
                            _context.SaveChanges();
                            return ApiResponse(
                                success: true,
                                message: "Cab Updated Successfully",
                                data: cab
                                );
                        }

                    }
                    else
                    {
                        return ApiResponse(
                            success: false,
                            message:"Record Not Found",
                            statusCode:404
                        );
                    }
                }
                catch (Exception ex)
                {
                    return ApiResponse(
                        success:false,
                        message:"Something Went Wrong",
                        error:ex.Message,
                        statusCode:500
                        );
                }
            }

            
            // ======================
            // DELETE (SOFT DELETE)
            // ======================
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteCab(int id)
            {
                try
                {
                    var cab = await _context.Cabs
                    .FirstOrDefaultAsync(c => c.CabId.Equals(id) && c.IsDeleted.Equals(false));

                    if (cab == null)
                    {
                        return ApiResponse(
                            success:false,
                            message:"Record Not Found",
                            statusCode:400
                            );
                    }
                    else
                    {
                        cab.IsDeleted = true;
                        _context.SaveChanges();

                    }
                    return ApiResponse(
                        success:true, 
                        message:"Record Deleted Successfully"
                        );
                }
                catch (Exception ex)
                {
                    return ApiResponse(
                        success:false, 
                        message:"Something Went Wrong",
                        error:ex.Message,
                        statusCode:500
                        );
                }
            }



        }
    }

