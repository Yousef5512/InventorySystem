using Inventory.Dal.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signIn;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(SignInManager<AppUser> signIn, UserManager<AppUser> userManager)
        {
            _signIn = signIn;
            _userManager = userManager;
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            var res = await _signIn.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
            if (!res.Succeeded) return Unauthorized("Invalid credentials");
            return Ok("Logged in");
        }

        [HttpGet("whoami")]
        [Authorize]
        public async Task<IActionResult> WhoAmI()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.UserName, Roles = roles });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return Ok();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] string username, [FromForm] string email, [FromForm] string password, [FromForm] string fullName)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
                return BadRequest("Username already exists");

            var user = new AppUser
            {
                UserName = username,
                Email = email,
                FullName = fullName 
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Optionally add default role
            await _userManager.AddToRoleAsync(user, "User");

            return Ok("User registered successfully");
        }
    }
}
