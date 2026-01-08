using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================
        // 1️⃣ GET ALL ROLES
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.RoleName)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return ApiResponse(true, "Roles retrieved successfully", roles);
        }

        // =========================================
        // 2️⃣ GET ROLE BY ID
        // =========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(short id)
        {
            var role = await _context.Roles
                .Where(r => r.RoleId == id && !r.IsDeleted)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    IsActive = r.IsActive
                })
                .FirstOrDefaultAsync();

            if (role == null)
                return ApiResponse(false, "Role not found", null, "not_found", null, 404);

            return ApiResponse(true, "Role retrieved successfully", role);
        }

        // =========================================
        // 3️⃣ CREATE ROLE
        // =========================================
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoleName))
                return ApiResponse(false, "RoleName is required", null, "validation_error",
                    new List<string> { "RoleName cannot be empty" }, 400);

            bool exists = await _context.Roles
                .AnyAsync(r => r.RoleName == dto.RoleName && !r.IsDeleted);

            if (exists)
                return ApiResponse(false, "Role already exists", null, "duplicate", null, 409);

            var role = new Role
            {
                RoleName = dto.RoleName,
                IsActive = true,
                IsDeleted = false
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return ApiResponse(true, "Role created successfully", role, null, null, 201);
        }

        // =========================================
        // 4️⃣ UPDATE ROLE
        // =========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(short id, [FromBody] UpdateRoleDTO dto)
        {
            if (id != dto.RoleId)
                return ApiResponse(false, "RoleId mismatch", null, "bad_request", null, 400);

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id && !r.IsDeleted);

            if (role == null)
                return ApiResponse(false, "Role not found", null, "not_found", null, 404);

            bool duplicate = await _context.Roles
                .AnyAsync(r => r.RoleName == dto.RoleName &&
                               r.RoleId != id &&
                               !r.IsDeleted);

            if (duplicate)
                return ApiResponse(false, "Role name already exists", null, "duplicate", null, 409);

            role.RoleName = dto.RoleName;
            role.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Role updated successfully", role);
        }

        // =========================================
        // 5️⃣ DELETE ROLE (SOFT DELETE)
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(short id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id && !r.IsDeleted);

            if (role == null)
                return ApiResponse(false, "Role not found", null, "not_found", null, 404);

            role.IsDeleted = true;
            role.IsActive = false;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Role deleted successfully");
        }
    }
}
