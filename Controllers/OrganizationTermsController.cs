using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationTermsController : BaseApiController
    {
       ApplicationDbContext _context;

        public OrganizationTermsController( ApplicationDbContext context)
        {
           _context = context;
        }

        // ------------------------------
        // GET ALL ORGANIZATIONTERMS
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllOrganizationTerms()
        {
            try
            {
                var terms = await _context.OrganizationTerms
                                   .Where(o=>o.IsDeleted.Equals(false))
                                   .ToListAsync();

                return ApiResponse(true,"OrganizationTerm Fetched Successfully",terms);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }

        // ------------------------------
        // GET ORGANIZATIONTERMS BY ID
        // ---------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganizationTerm(int id)
        {
            try
            {
                var term = await _context.OrganizationTerms.FirstOrDefaultAsync(o => o.OrganizationtermId == id && o.IsDeleted == false);

                if (term == null)
                {
                    return ApiResponse(false, "OrganizationTerm Not Found", 400);
                }

                return ApiResponse(true, "OrganizationTerm Fetched Successfully", term);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }

        // ------------------------------
        // CREATE ORGANIZATIONTERM
        // ------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateOrganizationTerm([FromForm] CreateTermDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse(false, "OrganizationTerm Not Found", 400);
                    
                   
               }

                OrganizationTerm term = new OrganizationTerm()
                {
                    OrganizationtermId = dto.OrganizationtermId,
                    OrganizationId = dto.OrganizationId,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                await _context.OrganizationTerms.AddAsync(term);
                await _context.SaveChangesAsync();


                return ApiResponse(true, "OrganizationTerm Added Successfully", term);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);

            }
        }

        // ------------------------------
        // UPDATE ORGANIZATIONTERM BY ID
        // ------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganizationTerm(int id, [FromForm] UpdateTermDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse(false, "Invalid Validation", 400);

                }

                var term = await _context.OrganizationTerms
                    .FirstOrDefaultAsync(x => x.OrganizationtermId.Equals(id) && x.IsDeleted.Equals(false));

                if (term == null)
                {
                    return ApiResponse(false, "OrganizationTerm Not Found", 400);

                }
                else
                {
                    term.OrganizationId = dto.OrganizationId;
                    term.OrganizationtermId = dto.OrganizationtermId;
                    term.Description = dto.Description;
                    term.UpdatedAt = DateTime.Now;
                    term.IsDeleted = false;

                    await _context.SaveChangesAsync();


                    return ApiResponse(
                        success: true,
                        message: "OrganizationTerm  Updated Successfully"
                        );
                }

               
            }
            catch (Exception ex)
            {
                return ApiResponse(
                    success:false,
                    message:"Internal Server Error",
                    statusCode:500
                    );

            }
        }

        // ------------------------------
        // DELETE ORGANIZATIONTERM BY ID  (SOFT DELETE)
        // ------------------------------

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganizationTerm(int id)
        {
            try
            {
                OrganizationTerm organizationTerm = await _context.OrganizationTerms.FirstOrDefaultAsync(t => t.OrganizationtermId == id && t.IsDeleted == false);

                if (organizationTerm == null)
                {
                    return ApiResponse(false, "Invalid Validation", 400);
                }

                organizationTerm.IsDeleted = true;
                await _context.SaveChangesAsync();

                return ApiResponse(true, "OrganizationTerm Delete Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something Went Wrong", ex.Message);
            }
        }
    }
}

