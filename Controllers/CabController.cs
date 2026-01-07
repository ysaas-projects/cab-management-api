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


            [HttpGet]
            [Route("")]
            public async Task<IActionResult> FetchCabs()
            {
                try
                {

                    var cabs = await _context.Cabs
                    .Where(c => c.IsDeleted != true)
                    .ToListAsync();

                    return ApiResponse(
                        success: true,
                        message:"Cabs Fetched Succefully",
                        data:cabs
                        );
                }
                catch (Exception ex)
                {
                    return ApiResponse(
                        success:false,
                        message:"Something Went wrong",
                        error:ex.Message,
                        statusCode:500
                        );
                }
            }


            [HttpGet]
            [Route("{id}")]
            public async Task<IActionResult> GetById(int id)
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

            [HttpPost]
            [Route("create")]
            public async Task<IActionResult> CreateData([FromForm] CreateCabDto cab)
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
                        message:"validation failed",
                        errors:errorlst,
                        statusCode:400
                        );
                    }

                    Cab cabdata = new Cab()
                    {
                        OrganizationId = cab.OrganizationId,
                        CabType = cab.CabType,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        IsActive = cab.IsActive
                    };

                    await _context.Cabs.AddAsync(cabdata);
                    _context.SaveChanges();

                    return ApiResponse(
                    success: true,
                    message: "Record Added SuccessFully",
                    data:cabdata
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

            [HttpPut]
            [Route("update{id}")]
            public async Task<IActionResult> UpdateData(int id, [FromForm]UpdateCabDto cab)
            {

                try
                {
                    var cabdata = await _context.Cabs.
                    FindAsync(id);
                    if(cabdata!=null)
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
                            cabdata.OrganizationId = cab.OrganizationId;
                            cabdata.CabType = cab.CabType;
                            cabdata.UpdetedAt = DateTime.Now;
                            cabdata.IsDeleted = cab.IsDeleted;
                            cabdata.IsActive = cab.IsActive;
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


            [HttpDelete]
            [Route("delete/{id}")]
            public async Task<IActionResult> DeleteData(int id)
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

