using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public OrganizationsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ------------------------------
        // GET ALL ORGANIZATIONS
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> GetOrganizations()
        {
            try
            {
                var organizations = await _context.organizations
                    .Where(o => !o.IsDeleted)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return ApiResponse(true, "Organizations retrieved successfully", organizations);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // ------------------------------
        // GET ORGANIZATION BY ID
        // ------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganizationById(int id)
        {
            try
            {
                var organization = await _context.organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == id && !o.IsDeleted);

                if (organization == null)
                    return ApiResponse(false, "Organization not found", error: "NotFound");

                return ApiResponse(true, "Organization retrieved successfully", organization);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // ------------------------------
        // CREATE ORGANIZATION
        // ------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromForm] CreateOrganizationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                bool duplicate = await _context.organizations.AnyAsync(o =>
                    o.OrganizationName.ToLower() == dto.OrganizationName.ToLower() &&
                    !o.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Organization already exists", error: "Duplicate");

                string logoPath = null;

                if (dto.LogoImage != null && dto.LogoImage.Length > 0)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "images", "organizations");
                    Directory.CreateDirectory(folderPath);

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.LogoImage.FileName)}";
                    string fullPath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.LogoImage.CopyToAsync(stream);

                    logoPath = $"images/organizations/{fileName}";
                }

                var organization = new Organization
                {
                    OrganizationName = dto.OrganizationName.Trim(),
                    Address = dto.Address,
                    MobileNumber = dto.MobileNumber,
                    GstNumber = dto.GstNumber,
                    LogoImagePath = logoPath,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.organizations.Add(organization);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Organization created successfully", organization);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // ------------------------------
        // UPDATE ORGANIZATION
        // ------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(int id, [FromForm] UpdateOrganizationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                var organization = await _context.organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == id && !o.IsDeleted);

                if (organization == null)
                    return ApiResponse(false, "Organization not found", error: "NotFound");

                bool duplicate = await _context.organizations.AnyAsync(o =>
                    o.OrganizationName.ToLower() == dto.OrganizationName.ToLower() &&
                    o.OrganizationId != id &&
                    !o.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Organization already exists", error: "Duplicate");

                if (dto.LogoImage != null && dto.LogoImage.Length > 0)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "images", "organizations");
                    Directory.CreateDirectory(folderPath);

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.LogoImage.FileName)}";
                    string fullPath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.LogoImage.CopyToAsync(stream);

                    organization.LogoImagePath = $"images/organizations/{fileName}";
                }

                organization.OrganizationName = dto.OrganizationName.Trim();
                organization.Address = dto.Address;
                organization.MobileNumber = dto.MobileNumber;
                organization.GstNumber = dto.GstNumber;
                organization.IsActive = dto.IsActive;
                organization.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Organization updated successfully", organization);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // ------------------------------
        // DELETE ORGANIZATION (SOFT DELETE)
        // ------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            try
            {
                var organization = await _context.organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == id && !o.IsDeleted);

                if (organization == null)
                    return ApiResponse(false, "Organization not found", error: "NotFound");

                organization.IsDeleted = true;
                organization.IsActive = false;
                organization.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Organization deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }
    }
}
