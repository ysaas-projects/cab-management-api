using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public UserRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================
        //  GET ALL USER ROLES
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => !ur.IsDeleted)
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
        //  GET USER ROLE BY ID
        // =========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleById(int id)
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserRoleId == id && !ur.IsDeleted)
                .Select(ur => new UserRoleDTO
                {
                    UserRoleId = ur.UserRoleId,
                    UserId = ur.UserId,
                    UserName = ur.User.UserName,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.RoleName,
                    IsActive = ur.IsActive,
                    CreatedAt = ur.CreatedAt,
                    UpdatedAt = ur.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (userRole == null)
                return ApiResponse(false, "User role not found", null, "not_found", null, 404);

            return ApiResponse(true, "User role retrieved successfully", userRole);
        }

        // =========================================
        //  CREATE USER ROLE
        // =========================================
        [HttpPost]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ApiResponse(false, "Invalid data", null, "validation_error", errors, 400);
            }

            bool exists = await _context.UserRoles.AnyAsync(ur =>
                ur.UserId == dto.UserId &&
                ur.RoleId == dto.RoleId &&
                !ur.IsDeleted);

            if (exists)
                return ApiResponse(false, "User already has this role", null, "duplicate", null, 409);

            var userRole = new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId,
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
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO dto)
        {
            if (id != dto.UserRoleId)
                return ApiResponse(false, "UserRoleId mismatch", null, "bad_request", null, 400);

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserRoleId == id && !ur.IsDeleted);

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
        //  DELETE USER ROLE 
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserRoleId == id && !ur.IsDeleted);

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
