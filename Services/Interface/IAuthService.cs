using cab_management.Models;

namespace cab_management.Services.Interface
{
	public interface IAuthService
	{
		Task<User> ValidateUser(string username, string password);
		Task<User> GetUserById(int userId);
		Task UpdateUserLastLogin(int userId);
		Task RevokeRefreshToken(string refreshToken);
		Task<TokenPair?> RefreshTokenAsync(string refreshToken);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    }

}
