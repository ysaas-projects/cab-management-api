// UserService.cs
using Microsoft.EntityFrameworkCore;
using cab_management.Data;
using cab_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cab_management.Services.Interface;

namespace cab_management.Services
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;
		private readonly IAuthService _authService;

		public UserService(ApplicationDbContext context, IAuthService authService)
		{
			_context = context;
			_authService = authService;
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			return await _context.Users
				.Include(u => u.Firm)
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role)
				.FirstOrDefaultAsync(u => u.UserId == userId);
		}

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    MobileNumber = u.MobileNumber,
                    IsActive = u.IsActive,
                    FirmId = u.FirmId,
                    Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList()
                })
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user, string password, short? roleId = null)
		{
			if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
				throw new Exception("Username already exists");

			if (!string.IsNullOrEmpty(user.Email) &&
				await _context.Users.AnyAsync(u => u.Email == user.Email))
				throw new Exception("Email already exists");

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
			user.SecurityStamp = Guid.NewGuid();

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			if (roleId.HasValue)
			{
				await AssignRoleToUserAsync(user.UserId, roleId.Value);
			}

			return user;
		}

		public async Task<User> UpdateUserAsync(int userId, User user)
		{
			var existingUser = await GetUserByIdAsync(userId);
			if (existingUser == null)
				throw new Exception("User not found");

			if (existingUser.UserName != user.UserName &&
				await _context.Users.AnyAsync(u => u.UserName == user.UserName))
				throw new Exception("Username already exists");

			if (!string.IsNullOrEmpty(user.Email) &&
				existingUser.Email != user.Email &&
				await _context.Users.AnyAsync(u => u.Email == user.Email))
				throw new Exception("Email already exists");

			// Update properties
			existingUser.UserName = user.UserName;
			existingUser.Email = user.Email;
			existingUser.MobileNumber = user.MobileNumber;
			existingUser.FirmId = user.FirmId;
			existingUser.IsActive = user.IsActive;
			existingUser.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
			return existingUser;
		}

		public async Task<bool> DeleteUserAsync(int userId)
		{
			var user = await GetUserByIdAsync(userId);
			if (user == null)
				return false;

			// Soft delete
			user.IsDeleted = true;
			user.UpdatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
		{
			var user = await GetUserByIdAsync(userId);
			if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
				return false;

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
			user.SecurityStamp = Guid.NewGuid(); // Invalidate existing sessions/tokens
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
		{
			var user = await GetUserByIdAsync(userId);
			if (user == null)
				return false;

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
			user.SecurityStamp = Guid.NewGuid(); // Invalidate existing sessions/tokens
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<User> CreateAdminUserAsync(string username, string password, string email)
		{
			// Check if admin role exists
			var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "admin");
			if (adminRole == null)
			{
				adminRole = new Role { RoleName = "admin", IsActive = true };
				_context.Roles.Add(adminRole);
				await _context.SaveChangesAsync();
			}

			var user = new User
			{
				UserName = username,
				Email = email,
				EmailConfirmed = true,
				IsActive = true
			};

			return await CreateUserAsync(user, password, adminRole.RoleId);
		}

		public async Task<bool> AssignRoleToUserAsync(int userId, short roleId)
		{
			// Check if user already has this role
			if (await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId))
				return false;

			var userRole = new UserRole
			{
				UserId = userId,
				RoleId = roleId,
				IsActive = true
			};

			_context.UserRoles.Add(userRole);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RemoveRoleFromUserAsync(int userId, short roleId)
		{
			var userRole = await _context.UserRoles
				.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

			if (userRole == null)
				return false;

			_context.UserRoles.Remove(userRole);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<Role>> GetUserRolesAsync(int userId)
		{
			return await _context.UserRoles
				.Where(ur => ur.UserId == userId)
				.Select(ur => ur.Role)
				.ToListAsync();
		}

		public async Task<bool> ToggleUserStatusAsync(int userId, bool isActive)
		{
			var user = await GetUserByIdAsync(userId);
			if (user == null)
				return false;

			user.IsActive = isActive;
			user.UpdatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return true;
		}
	}
}