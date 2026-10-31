using ErpForSchoolCare.Models;

namespace ErpForSchoolCare
{
    public static class AuthCookieHelper
    {
        public static void SetAuthCookies(HttpContext context, LoginResponseDto token)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // HTTPS only
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            context.Response.Cookies.Append("access_token", token.AccessToken, options);
            context.Response.Cookies.Append("refresh_token", token.RefreshToken, options);
        }

        public static void ClearAuthCookies(HttpContext context)
        {
            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");
        }
    }

}
