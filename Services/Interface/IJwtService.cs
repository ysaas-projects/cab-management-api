using cab_management.Models;
using System.Security.Claims;

namespace cab_management.Services.Interface
{
	public interface IJwtService
	{
		//string GenerateAccessToken(User user, IList<string> roles);
		string GenerateRefreshToken();
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
		int GetRefreshTokenExpiryDays();
		int GetAccessTokenExpiryMinutes();
		(string Token, string Jti) GenerateAccessTokenWithJti(User user, List<string> roles);

	}
}
