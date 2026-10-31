using ErpForSchoolCare.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace ErpForSchoolCare.Filter
{
    public class TokenPreCheckFilter : IAsyncAuthorizationFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenPreCheckFilter(IHttpClientFactory httpClientFactory,IHttpContextAccessor httpContextAccessor)
        {
            this._httpClientFactory = httpClientFactory;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            var accessToken = httpContext.Request.Cookies["access_token"];
            var refreshToken = httpContext.Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                httpContext.Response.StatusCode=StatusCodes.Status401Unauthorized;
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);

            if (jwt.ValidTo > DateTime.UtcNow)
                return;

            var client = _httpClientFactory.CreateClient("AuthClient");

            var response = await client.PostAsJsonAsync(
                "Auth/refresh-token",
                new { refreshToken });

            if (!response.IsSuccessStatusCode)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }                

            var newToken =
                await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            if (newToken == null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            AuthCookieHelper.SetAuthCookies(httpContext, newToken);
            httpContext.Items["New_Access_Token"] = newToken.AccessToken;
            await Utility.SignInMvcUser(httpContext, newToken);
        }
    }
}
