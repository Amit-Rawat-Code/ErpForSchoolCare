using ErpForSchoolCare.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ErpForSchoolCare
{
    public static class Utility
    {
        public static async Task SignInMvcUser(HttpContext context,LoginResponseDto token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token.AccessToken);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,
                        jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),

                    new Claim(ClaimTypes.Name,
                        jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value),

                    new Claim(ClaimTypes.Role,
                        jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value),
                };
            var exp = jwt.Claims.First(c => c.Type == "exp").Value;

            claims.Add(new Claim("exp", exp));
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15)
                });
        }
    }
}
