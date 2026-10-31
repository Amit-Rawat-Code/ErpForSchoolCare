using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApis.Entities;
using SchoolApis.Model;
using SchoolApis.Services;
using System.Security.Claims;

namespace SchoolApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {        
        private readonly IAuthService authService;
        private readonly IAuthCookieService authCookieService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService,IAuthCookieService authCookieService,IConfiguration config)
        {
            this.authService = authService;
            this.authCookieService = authCookieService;
            this._config = config;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User?>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("User already exists");
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<RefreshTokenResponseDto>> LoginAsync(UserDto request)
        {
            var token= await authService.LoginAsync(request);
            if (token is null)
                return Unauthorized("Wrong UserName or Password");
            authCookieService.SetAuthCookies(Response, token.AccessToken, token.RefreshToken);
            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto? dto)
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                refreshToken = dto?.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh Token missing");
            var token = await authService.RefreshTokenAsync(refreshToken);
            if (token is null)
                return Unauthorized("Invalid/Expired token");
            authCookieService.SetAuthCookies(Response, token.AccessToken, token.RefreshToken);
            return Ok(token);
        }

        [HttpGet("Auth-endpoint")]
        [Authorize]
        public ActionResult AuthCheck()
        {
            return Ok();
        }

        [HttpGet("Admin-endpoint")]
        [Authorize(Roles ="Admin")]
        public ActionResult AdminCheck()
        {
            return Ok();
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            //return Ok(new
            //{
            //    userName = User.Identity!.Name,
            //    expiresAt = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Coockie:AccessTokenDuration")) // or from token
            //});
            var expClaim = User.FindFirst("exp")?.Value;

            if (expClaim == null)
                return Unauthorized();

            var expiryUtc = DateTimeOffset
                .FromUnixTimeSeconds(long.Parse(expClaim))
                .UtcDateTime;

            return Ok(new
            {
                userName = User.Identity!.Name,
                expiresAt = expiryUtc
            });
        }
        [Authorize]
        [HttpGet("is-authenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await authService.LogoutAsync(userId);
            authCookieService.ClearAuthCookies(Response);

            return Ok();
        }


    }
}
