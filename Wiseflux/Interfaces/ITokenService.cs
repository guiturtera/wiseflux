using System.Security.Claims;
using Wiseflux.Models;

namespace Wiseflux.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User claims, DateTime expiryDate);
        string GenerateRefreshToken();
        ClaimsPrincipal GetUserFromExpiredToken(string token);
    }
}
