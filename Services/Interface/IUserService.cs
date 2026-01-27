// IUserService.cs
using cab_management.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cab_management.Services
{
	public interface IUserService
	{
		Task<User> GetUserByIdAsync(int userId);
		Task<List<User>> GetAllUsersAsync();
		Task<User> CreateUserAsync(User user, string password, short? roleId = null);
		Task<User> UpdateUserAsync(int userId, User user);
		Task<bool> DeleteUserAsync(int userId); 
		Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
		Task<bool> ResetPasswordAsync(int userId, string newPassword);
		Task<User> CreateAdminUserAsync(string username, string password, string email);
		Task<bool> AssignRoleToUserAsync(int userId, short roleId);
		Task<bool> RemoveRoleFromUserAsync(int userId, short roleId);
		Task<List<Role>> GetUserRolesAsync(int userId);
		Task<bool> ToggleUserStatusAsync(int userId, bool isActive);
	}
}