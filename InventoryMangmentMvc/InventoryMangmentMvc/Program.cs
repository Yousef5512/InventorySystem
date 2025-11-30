using InventoryMangmentMvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var cookieContainer = new CookieContainer();
builder.Services.AddSingleton(cookieContainer);


builder.Services.AddControllersWithViews();

// Configure HttpClient with cookie handler
builder.Services.AddHttpClient("InventoryAPI", (sp, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7024/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
{
    UseCookies = true,
    CookieContainer = sp.GetRequiredService<CookieContainer>(),
    AllowAutoRedirect = false
});

builder.Services.AddScoped<IApiService, ApiService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "InventoryAuth";
        options.LoginPath = "/Account/Login";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();