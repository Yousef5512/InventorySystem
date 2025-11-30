using Inventory.Bll.Contract;
using Inventory.Bll.DTOs;
using Inventory.Bll.services;
using Inventory.Dal.Entities;
using Inventory.Dal.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace InventoryMangmentApi.Controllers
{
    [ApiController]
    [Route("api/primary")]
    [Authorize(Roles = "PrimaryUser")]
    public class PrimaryController : ControllerBase
    {
        private readonly IOrderServices _orderService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProductServices _productService;
    

        public PrimaryController(
            IOrderServices orderService,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,IProductServices productServices)
        {
            _orderService = orderService;
            _userManager = userManager;
            _roleManager = roleManager;
            _productService = productServices;
          
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found");
            return Ok(product);
        }
        // New endpoint: assign role to a user
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return NotFound("User not found");

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            // remove old roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign new role
            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new { Username = user.UserName, AssignedRole = dto.Role });
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var product = await _productService.CreateProductAsync(dto);
            return Ok(product);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCreateDto dto)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found");

            product.ProductName = dto.ProductName;
            product.Quantity = dto.Quantity;
            product.Material = dto.Material;
            product.Size = dto.Size;
            product.Flavour = dto.Flavour;
            product.Type = dto.Type;
            product.ExpirationDate = dto.ExpirationDate;

            await _productService.UpdateAsync(product);
            return Ok(product);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found");

            await _productService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> SendProducts([FromBody] List<ProductSendDetailDto> products)
        {
            var send = await _productService.SendProductsToInventoryAsync(products);
            return Ok(send);
        }
        [HttpDelete("delete-user/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Username is required");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound($"User '{username}' not found");

            // Optional safety: prevent deleting self
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.UserName == username)
                return BadRequest("You cannot delete your own account");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"User '{username}' has been deleted successfully");
        }



    }
}
