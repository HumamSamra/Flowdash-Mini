using System.Security.Claims;

namespace Flowdash_Mini.Extensions
{
    public static class PrincipalExtension
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
        }
        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        }
        public static string? Name(this ClaimsPrincipal user)
        {
            return user.Identity?.Name;
        }
    }
}
