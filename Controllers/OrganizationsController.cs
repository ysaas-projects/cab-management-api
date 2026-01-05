using cab_management.Data;
using cab_management.Models;
using cab_management.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme + "," +
            CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public OrganizationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // GET ALL ORGANIZATIONS
        // ================================
        [HttpGet]
        public async Task<IActionResult> GetOrganisation()
        {
            try
            {
                var organizations = await _context.Organizations
                    .Where(o => !o.IsDeleted)
                    .OrderBy(o => o.OrganizationName)
                    .Select(o => new OrganizationResponseDto
                    {
                        //OrganizationId = o.OrganizationId,
                        OrganizationName = o.OrganizationName,
                        Address = o.Address,
                        MobileNumber = o.MobileNumber,
                        GstNumber = o.GstNumber,
                        IsActive = o.IsActive,
                        IsDeleted = o.IsDeleted
                    })
                    .ToListAsync();

                return ApiResponse(true, "Organizations retrieved successfully", organizations);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving organizations", error: ex.Message);
            }
        }

        // ================================
        // CREATE ORGANIZATION
        // ================================
        [HttpPost]
      
        public async Task<IActionResult> CreateOrganization([FromForm] OrganizationCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ApiResponse(false, "Invalid data", null, "bad_request", errors);
            }

            try
            {
                bool exists = await _context.Organizations.AnyAsync(o =>
                    o.OrganizationName.ToLower() == dto.OrganizationName.ToLower() && !o.IsDeleted);

                if (exists)
                    return ApiResponse(false, "Organization already exists", error: "Duplicate");

                string logoPath = null;

                if (dto.LogoImage != null && dto.LogoImage.Length > 0)
                {
                    // sanitize organization name for folder name (remove spaces, special chars)
                    var folderName = dto.OrganizationName.Replace(" ", "_");
                    var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

                    if (!Directory.Exists(uploadsRootFolder))
                    {
                        Directory.CreateDirectory(uploadsRootFolder);
                    }

                    var fileName = Path.GetFileName(dto.LogoImage.FileName);
                    var filePath = Path.Combine(uploadsRootFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.LogoImage.CopyToAsync(stream);
                    }

                    // Save relative path to DB (relative to wwwroot)
                    logoPath = Path.Combine("uploads", folderName, fileName).Replace("\\", "/");
                }

                var organization = new Organization
                {
                    OrganizationName = dto.OrganizationName.Trim(),
                    LogoImagePath = logoPath,
                    Address = dto.Address,
                    MobileNumber = dto.MobileNumber,
                    GstNumber = dto.GstNumber,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Organization created successfully", new
                {
                    organization.OrganizationId,
                    organization.OrganizationName
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error creating organization", error: ex.Message);
            }
        }

        // ================================
        // UPDATE ORGANIZATION
        // ================================
        [HttpPut("{id}")]
      
        public async Task<IActionResult> UpdateOrganization(
            int id,
            [FromBody] OrganizationUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ApiResponse(false, "Invalid data", null, "bad_request", errors);
            }

            try
            {
                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == id && !o.IsDeleted);

                if (organization == null)
                    return ApiResponse(false, "Organization not found", error: "NotFound");

                bool duplicate = await _context.Organizations.AnyAsync(o =>
                    o.OrganizationName.ToLower() == dto.OrganizationName.ToLower()
                    && o.OrganizationId != id
                    && !o.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Organization already exists", error: "Duplicate");

                organization.OrganizationName = dto.OrganizationName.Trim();
                organization.LogoImagePath = dto.LogoImagePath;
                organization.Address = dto.Address;
                organization.MobileNumber = dto.MobileNumber;
                organization.GstNumber = dto.GstNumber;
                organization.IsActive = dto.IsActive;
                organization.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Organization updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error updating organization", error: ex.Message);
            }
        }
    }
}
