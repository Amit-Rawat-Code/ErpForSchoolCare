using ErpForSchoolCare.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace ErpForSchoolCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(User user)
        {
            var client = httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("Auth/login", user);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid login");
                return View(user);
            }
            var token = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            AuthCookieHelper.SetAuthCookies(HttpContext, token!);
            await Utility.SignInMvcUser(HttpContext, token!);
            return RedirectToAction("Dashboard", "DashBoard");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("Auth/Register", user);
            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Registered Succesfully!";
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("Error", "Login failed");
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            AuthCookieHelper.ClearAuthCookies(HttpContext);
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
        //[Authorize]
        //public async Task<IActionResult> SessionInfo()
        //{
        //    var client = httpClientFactory.CreateClient("ApiClient");
        //    var response = await client.GetAsync("Auth/me");

        //    if (!response.IsSuccessStatusCode)
        //        return Unauthorized();

        //    var data = await response.Content.ReadFromJsonAsync<object>();
        //    return Json(data);
        //}
        [Authorize]
        public IActionResult SessionInfo()
        {
            var expClaim = User.FindFirst("exp")?.Value;

            if (!long.TryParse(expClaim, out var seconds))
                return Unauthorized();

            var expiresAt = DateTimeOffset
                .FromUnixTimeSeconds(seconds)
                .UtcDateTime;

            return Json(new { expiresAt });
        }
    }
}
