using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolApis.Data;
using SchoolApis.Entities;
using SchoolApis.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolApis.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly MyDbContext context;

        public AuthService(IConfiguration configuration, MyDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.UserName == request.UserName))
                return null;
            var user = new User();
            user.UserName = request.UserName;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }
        public async Task<RefreshTokenResponseDto?> LoginAsync(UserDto request)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
                return null;
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                return null;
            var token = new RefreshTokenResponseDto
            {
                AccessToken = CreatedToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
            return token;
        }

        public async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            var refreshToken = Convert.ToBase64String(random);
            user.RefreshToken=refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            await context.SaveChangesAsync();
            return refreshToken;
        }
        private string CreatedToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Roles),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<RefreshTokenResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            //var user = await context.Users.FindAsync(request.Id);
            //if(user is null||user.RefreshToken!=request.RefreshToken||user.RefreshTokenExpiry<DateTime.UtcNow)
            //    return null;
            //var token = new RefreshTokenResponseDto
            //{
            //    AccessToken = CreatedToken(user),
            //    RefreshToken = await GenerateAndSaveRefreshToken(user),
            //};
            //return token;
            var user = await context.Users
        .FirstOrDefaultAsync(u =>
            u.RefreshToken == refreshToken &&
            u.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return null;

            return new RefreshTokenResponseDto
            {
                AccessToken = CreatedToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
        }
        public async Task LogoutAsync(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await context.SaveChangesAsync();
        }

    }
}
