using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ErpForSchoolCare
{
    public class UserContext
    {
        public Guid? UserId { get; }
        public string? UserName { get; }
        public string[] Roles { get; }
        public DateTime? TokenExpiry { get; }

        public UserContext(IHttpContextAccessor accessor)
        {
            var user = accessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                Roles = Array.Empty<string>();
                return;
            }

            // User Id (GUID)
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var id))
                UserId = id;

            // User Name
            UserName = user.Identity?.Name;

            // Roles (multiple supported)
            Roles = user.FindAll(ClaimTypes.Role)
                        .Select(r => r.Value)
                        .ToArray();

            // Token expiry (JWT exp claim → UTC DateTime)
            var exp = user.FindFirst("exp")?.Value;
            if (long.TryParse(exp, out var seconds))
            {
                TokenExpiry = DateTimeOffset
                    .FromUnixTimeSeconds(seconds)
                    .UtcDateTime;
            }
        }
    }
}
