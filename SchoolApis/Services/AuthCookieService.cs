namespace SchoolApis.Services
{
    public class AuthCookieService : IAuthCookieService
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration _config;
        public AuthCookieService(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config=config;
        }

        public void SetAuthCookies(
            HttpResponse response,
            string accessToken,
            string refreshToken)
        {
            var isDev = _env.IsDevelopment();

            response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
                Secure = !isDev,
                Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Coockie:AccessTokenDuration"))
            });

            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
                Secure = !isDev,
                Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Coockie:RefreshTokenDuration"))
            });
        }

        public void ClearAuthCookies(HttpResponse response)
        {
            response.Cookies.Delete("access_token");
            response.Cookies.Delete("refresh_token");
        }
    }

}
