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

                    return ApiResponse(true, "Cabs Fetched Succefully", cabs);
                }
                catch (Exception ex)
                {
                    return ApiResponse(false, "Something Went wrong", ex.Message);
                }
            }


            [HttpGet]
            [Route("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                try
                {
                    var c = _context.Cabs.
                    FirstOrDefault(x => x.CabId.Equals(id) && x.IsDeleted.Equals(false));

                    if (c != null)
                    {
                        return ApiResponse(true, "Cab Fetched Successfully", c);
                    }
                    else
                    {
                        return ApiResponse(false, "Record Not Found", 404);
                    }

                }
                catch (Exception ex)
                {
                    return ApiResponse(false, "Something Went Wrong", ex.Message);
                }
            }

            [HttpPost]
            [Route("create")]
            public async Task<IActionResult> CreateData([FromBody] CreateCabDto cab)
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

                    Cab c = new Cab()
                    {
                        OrganizationId = cab.OrganizationId,
                        CabType = cab.CabType,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        IsActive = cab.IsActive
                    };

                    await _context.Cabs.AddAsync(c);
                    _context.SaveChanges();
                    return ApiResponse(false, "Data Added SuccessFully", c);
                }
                catch (Exception ex)
                {
                    return ApiResponse(false, "Something Went Wrong", ex.Message);
                }
            }

            [HttpPut]
            [Route("update{id}")]
            public async Task<IActionResult> UpdateData(int id, UpdateCabDto cab)
            {

                try
                {
                    var c = await _context.Cabs.FindAsync(id);
                    if (!ModelState.IsValid && !ModelState.IsNullOrEmpty())
                    {
                        var errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(x => x.ErrorMessage)
                            .ToList();
                        return ApiResponse(false, "Validation Failed", errors);
                    }
                    else
                    {
                        c.OrganizationId = cab.OrganizationId;
                        c.CabType = cab.CabType;
                        c.UpdetedAt = DateTime.Now;
                        c.IsDeleted = cab.IsDeleted;
                        c.IsActive = cab.IsActive;
                    }
                    _context.SaveChanges();
                    return ApiResponse(true, "Cab Fetched Successfully", cab);
                }
                catch (Exception ex)
                {
                    return ApiResponse(false, "Something Went Wrong", ex.Message);
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
                        return ApiResponse(false, "Record Not Found", 400);
                    }
                    else
                    {
                        cab.IsDeleted = true;
                        _context.SaveChanges();

                    }
                    return ApiResponse(true, "Record Deleted Successfully");
                }
                catch (Exception ex)
                {
                    return ApiResponse(false, "Something Went Wrong", ex.Message);
                }
            }



        }
    }

