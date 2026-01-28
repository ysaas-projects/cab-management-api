using System.Security.Claims;
using cab_management.Data;
using cab_management.Models;
using cab_management.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cab_management.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
       JwtBearerDefaults.AuthenticationScheme + "," +
       CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseApiController
    { 
        
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpGet]
            [Authorize(Roles = "Super-Admin")]
            public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }

            [HttpGet("{userId}")]
            public async Task<ActionResult<User>> GetUserById(int userId)
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound();

                // Users can only access their own profile unless they're admin
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (currentUserId != userId && !User.IsInRole("Firm-Admin"))
                    return Forbid();

                return Ok(user);
            }

            [HttpPost]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateRequest request)
            {
                try
                {
                    var user = new User
                    {
                        UserName = request.Username,
                        Email = request.Email,
                        MobileNumber = request.MobileNumber,
                        FirmId = request.FirmId,
                        IsActive = request.IsActive
                    };

                    var createdUser = await _userService.CreateUserAsync(user, request.Password, request.RoleId);
                    return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.UserId }, createdUser);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpPost("admin")]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<ActionResult<User>> CreateAdminUser([FromBody] AdminUserCreateRequest request)
            {
                try
                {
                    var adminUser = await _userService.CreateAdminUserAsync(request.Username, request.Password, request.Email);
                    return CreatedAtAction(nameof(GetUserById), new { userId = adminUser.UserId }, adminUser);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpPut("{userId}")]
            public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateRequest request)
            {
                // Users can only update their own profile unless they're admin
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (currentUserId != userId && !User.IsInRole("Firm-Admin"))
                    return Forbid();

                try
                {
                    var user = new User
                    {
                        UserName = request.Username,
                        Email = request.Email,
                        MobileNumber = request.MobileNumber,
                        FirmId = request.FirmId,
                        IsActive = request.IsActive
                    };

                    var updatedUser = await _userService.UpdateUserAsync(userId, user);
                    return Ok(updatedUser);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpDelete("{userId}")]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<IActionResult> DeleteUser(int userId)
            {
                var result = await _userService.DeleteUserAsync(userId);
                if (!result)
                    return NotFound();

                return NoContent();
            }

        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordRequest request)
        {
            // Users can only change their own password unless they're admin
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (currentUserId != userId && !User.IsInRole("Firm-Admin"))
                return Forbid();

            var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (!result)
                return BadRequest("Current password is incorrect or user not found");

            return NoContent();
        }

        [HttpPost("{userId}/reset-password")]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<IActionResult> ResetPassword(int userId, [FromBody] Models.ResetPasswordRequest request)
            {
                var result = await _userService.ResetPasswordAsync(userId, request.NewPassword);
                if (!result)
                    return NotFound();

                return NoContent();
            }

            [HttpPost("{userId}/roles")]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleRequest request)
            {
                var result = await _userService.AssignRoleToUserAsync(userId, request.RoleId);
                if (!result)
                    return BadRequest("User already has this role");

                return NoContent();
            }

            [HttpDelete("{userId}/roles/{roleId}")]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<IActionResult> RemoveRole(int userId, short roleId)
            {
                var result = await _userService.RemoveRoleFromUserAsync(userId, roleId);
                if (!result)
                    return BadRequest("User doesn't have this role");

                return NoContent();
            }

            [HttpGet("{userId}/roles")]
            public async Task<ActionResult<IEnumerable<Role>>> GetUserRoles(int userId)
            {
                var roles = await _userService.GetUserRolesAsync(userId);
                return Ok(roles);
            }

            [HttpPost("{userId}/toggle-status")]
            [Authorize(Roles = "Firm-Admin")]
            public async Task<IActionResult> ToggleUserStatus(int userId, [FromBody] ToggleUserStatusRequest request)
            {
                var result = await _userService.ToggleUserStatusAsync(userId, request.IsActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }

        }
}
