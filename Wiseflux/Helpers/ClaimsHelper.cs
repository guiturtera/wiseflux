using System.Security.Claims;

namespace Wiseflux.Helpers
{
    public static class ClaimsHelper
    {
        public static string GetUserEmail(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).Value;
        }
    }
}
