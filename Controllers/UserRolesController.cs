using System.Security.Claims;
using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize]
    [Route("api/user-roles")]
    [ApiController]
    public class UserRolesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public UserRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // Helper: Get FirmId from JWT
        // ===============================
        private int GetFirmId()
        {
            return int.Parse(User.FindFirst("firmId")!.Value);
        }

        private bool IsSuperAdmin()
        {
            return User.IsInRole("Super-Admin");
        }

        // =========================================
        // GET ALL USER ROLES
        // Super-Admin → all firms
        // Firm-Admin  → own firm only
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            var firmId = GetFirmId();

            var query = _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => !ur.IsDeleted);

            if (!IsSuperAdmin())
            {
                query = query.Where(ur => ur.FirmId == firmId);
            }

            var userRoles = await query
                .Select(ur => new UserRoleDTO
                {
                    UserRoleId = ur.UserRoleId,
                    UserId = ur.UserId,
                    UserName = ur.User.UserName,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.RoleName,
                    IsActive = ur.IsActive,
                    IsDeleted = ur.IsDeleted,
                    CreatedAt = ur.CreatedAt,
                    UpdatedAt = ur.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse(true, "User roles retrieved successfully", userRoles);
        }

        // =========================================
        // GET USER ROLE BY ID
        // =========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleById(int id)
        {
            var firmId = GetFirmId();

            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur =>
                    ur.UserRoleId == id &&
                    !ur.IsDeleted &&
                    (IsSuperAdmin() || ur.FirmId == firmId)
                )
                .Select(ur => new UserRoleDTO
                {
                    UserRoleId = ur.UserRoleId,
                    UserId = ur.UserId,
                    UserName = ur.User.UserName,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.RoleName,
                    IsActive = ur.IsActive,
                    IsDeleted = ur.IsDeleted,
                    CreatedAt = ur.CreatedAt,
                    UpdatedAt = ur.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (userRole == null)
                return ApiResponse(false, "User role not found", null, "not_found", null, 404);

            return ApiResponse(true, "User role retrieved successfully", userRole);
        }

        // =========================================
        // CREATE USER ROLE
        // Firm-Admin / Super-Admin
        // =========================================
        [HttpPost]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(false, "Invalid data");

            var firmId = GetFirmId();

            // Ensure user belongs to same firm (unless Super-Admin)
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == dto.UserId &&
                    (IsSuperAdmin() || u.FirmId == firmId)
                );

            if (user == null)
                return ApiResponse(false, "User not found or invalid firm");

            bool exists = await _context.UserRoles.AnyAsync(ur =>
                ur.UserId == dto.UserId &&
                ur.RoleId == dto.RoleId &&
                ur.FirmId == firmId &&
                !ur.IsDeleted
            );

            if (exists)
                return ApiResponse(false, "User already has this role", null, "duplicate", null, 409);

            var userRole = new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                FirmId = firmId,             
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return ApiResponse(true, "User role created successfully", userRole, null, null, 201);
        }

        // =========================================
        // UPDATE USER ROLE
        // =========================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO dto)
        {
            if (id != dto.UserRoleId)
                return ApiResponse(false, "UserRoleId mismatch", null, "bad_request", null, 400);

            var firmId = GetFirmId();

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur =>
                    ur.UserRoleId == id &&
                    !ur.IsDeleted &&
                    (IsSuperAdmin() || ur.FirmId == firmId)
                );

            if (userRole == null)
                return ApiResponse(false, "User role not found", null, "not_found", null, 404);

            if (dto.UserId.HasValue)
                userRole.UserId = dto.UserId.Value;

            if (dto.RoleId.HasValue)
                userRole.RoleId = dto.RoleId.Value;

            if (dto.IsActive.HasValue)
                userRole.IsActive = dto.IsActive.Value;

            userRole.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "User role updated successfully", userRole);
        }

        // =========================================
        // DELETE USER ROLE (SOFT DELETE)
        // =========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            var firmId = GetFirmId();

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur =>
                    ur.UserRoleId == id &&
                    !ur.IsDeleted &&
                    (IsSuperAdmin() || ur.FirmId == firmId)
                );

            if (userRole == null)
                return ApiResponse(false, "User role not found", null, "not_found", null, 404);

            userRole.IsDeleted = true;
            userRole.IsActive = false;
            userRole.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "User role deleted successfully");
        }
    }
}
