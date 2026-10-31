using ErpForSchoolCare.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace ErpForSchoolCare.Handlers
{
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return await base.SendAsync(request, cancellationToken);

            // 1️. Attach current access token
            string? accessToken = null;
            if(context.Items.TryGetValue("New_Access_Token", out var tokenFromFilter))
            {
                accessToken=tokenFromFilter?.ToString();
            }
            else
            {
                accessToken = context.Request.Cookies["access_token"];
            }
            if (accessToken != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);
                var expiryLocal = jwt.ValidTo.ToLocalTime();
                string formatted = expiryLocal.ToString("dd-MM-yyyy HH:mm:ss");
            }
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

    public static class HttpContextTokenExtensions
    {
        private const string Key = "__TOKEN_REFRESHED__";

        public static bool IsTokenRefreshed(this HttpContext context)
            => context.Items.ContainsKey(Key);

        public static void MarkTokenRefreshed(this HttpContext context)
            => context.Items[Key] = true;
    }

    // HttpRequestMessage clone helper
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage Clone(this HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            clone.Content = request.Content;
            return clone;
        }
    }
}
