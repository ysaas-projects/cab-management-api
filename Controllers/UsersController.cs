using cab_management.Data;
using cab_management.Models;
using cab_management.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.UserName) 
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    FirmId = u.FirmId,
                    FirmType = u.FirmType,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    MobileNumber = u.MobileNumber,
                    MobileNumberConfirmed = u.MobileNumberConfirmed,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync();

            return ApiResponse(true, "Users retrieved successfully", users);
        }

        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordRequest request)

        {
            //var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (currentUserId != userId)
            //    return Forbid();

            var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (!result)
                return BadRequest("Current password is incorrect or user not found");

            return NoContent();
        }

    }
}
