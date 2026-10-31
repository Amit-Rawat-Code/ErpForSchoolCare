
namespace SchoolApis.Services
{
    public interface IAuthCookieService
    {
        void ClearAuthCookies(HttpResponse response);
        void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken);
    }
}