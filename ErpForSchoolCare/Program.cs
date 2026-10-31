using ErpForSchoolCare;
using ErpForSchoolCare.Filter;
using ErpForSchoolCare.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// MVC
// ------------------------------------
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews(options=>
    {
        options.Filters.Add<TokenPreCheckFilter>();
        options.Filters.Add<UnauthorizedRedirectFilter>();        
    })
    .AddRazorRuntimeCompilation();
}
else
{
    builder.Services.AddControllersWithViews(options=>
    {
        options.Filters.Add<TokenPreCheckFilter>();
        options.Filters.Add<UnauthorizedRedirectFilter>();
    });
}
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AccessTokenHandler>();
builder.Services.AddScoped<UserContext>();
// ------------------------------------
// Authentication (MVC UI SESSION ONLY)
// ------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";

        // Prevent 401 JSON response ? redirect instead
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.Redirect("/Account/Login");
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// ------------------------------------
// HTTP CLIENT (API)
// ------------------------------------
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
}).AddHttpMessageHandler<AccessTokenHandler>();

// Auth client WITHOUT handler (important)
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
});

// ------------------------------------
// Build
// ------------------------------------
var app = builder.Build();

// ------------------------------------
// Middleware pipeline
// ------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------
// Routing
// ------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DashBoard}/{action=Dashboard}/{id?}");

app.Run();
