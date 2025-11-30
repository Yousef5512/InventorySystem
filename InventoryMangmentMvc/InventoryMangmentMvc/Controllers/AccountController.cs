using InventoryMangmentMvc.Models;
using InventoryMangmentMvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var formData = new Dictionary<string, string>
                {
                    { "username", model.Username },
                    { "password", model.Password }
                };

                // Call login API - it returns "Logged in" on success
                var loginResponse = await _apiService.PostMultipartAsync<string>("account/login", formData);

                // Check if login was successful
                if (loginResponse != null && loginResponse.Contains("Logged in"))
                {
                    // Now call whoami to get user info and roles
                    try
                    {
                        var userInfo = await _apiService.GetAsync<UserInfoViewModel>("account/whoami");

                        if (userInfo != null && !string.IsNullOrEmpty(userInfo.UserName))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, userInfo.UserName)
                            };

                            // Handle roles - check both direct list and wrapper object
                            if (userInfo.Roles != null)
                            {
                                // If Roles is a wrapper with Values property
                                if (userInfo.Roles != null && userInfo.Roles.Any())
                                {
                                    foreach (var role in userInfo.Roles)
                                        claims.Add(new Claim(ClaimTypes.Role, role));
                                }
                            }

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                new AuthenticationProperties
                                {
                                    IsPersistent = true,
                                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                                }
                            );

                            return RedirectToAction("Dashboard");
                        }
                    }
                    catch (Exception whoamiEx)
                    {
                        // If whoami fails, still try to login with just username
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.Username),
                            // Add a default role to allow access
                            new Claim(ClaimTypes.Role, "InventoryEmployee")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                            }
                        );

                        return RedirectToAction("Dashboard");
                    }
                }

                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred during login: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var formData = new Dictionary<string, string>
                {
                    { "username", model.Username },
                    { "email", model.Email },
                    { "password", model.Password },
                    { "fullName", model.FullName }
                };

                var result = await _apiService.PostFormAsync<string>("account/register", formData);

                if (result != null && result.Contains("successfully"))
                {
                    TempData["SuccessMessage"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", result ?? "Registration failed. Username may already exist.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred during registration: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            var roles = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();

            if (roles.Contains("PrimaryUser"))
                return RedirectToAction("Index", "Primary");
            else if (roles.Contains("InventoryEmployee"))
                return RedirectToAction("Index", "InventoryEmployee");
            else if (roles.Contains("InventoryOrganizer"))
                return RedirectToAction("Index", "InventoryOrganizer");
            else if (roles.Contains("ReceivingClerk"))
                return RedirectToAction("Index", "ReceivingClerk");
            else if (roles.Contains("Supplier"))
                return RedirectToAction("Index", "Supplier");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await _apiService.PostAsync<object>("account/logout", null);
            }
            catch
            {
                
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}