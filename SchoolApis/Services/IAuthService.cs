using SchoolApis.Entities;
using SchoolApis.Model;

namespace SchoolApis.Services
{
    public interface IAuthService
    {
        Task<RefreshTokenResponseDto?> LoginAsync(UserDto request);
        Task<User?> RegisterAsync(UserDto request);
        Task<RefreshTokenResponseDto?> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
    }
}