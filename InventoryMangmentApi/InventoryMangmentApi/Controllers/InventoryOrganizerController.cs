using Inventory.Bll.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentApi.Controllers
{
    [ApiController]
    [Route("api/inventory-organizer")]
    [Authorize(Roles = "InventoryOrganizer")]
    public class InventoryOrganizerController : ControllerBase
    {
        private readonly IProductServices _productService;
        public InventoryOrganizerController(IProductServices productService) => _productService = productService;
        [HttpPost("assign-product-id/{productId}")]
        public async Task<IActionResult> AssignProductId(int productId)
        {
            var product = await _productService.GetAsync(productId);
            if (product == null)
                return NotFound("Product not found.");

            string code = $"{product.Material}-{product.Size}-{product.Type}-{product.Flavour}-{Guid.NewGuid().ToString()[..4]}";

            await _productService.AssignProductIdCodeAsync(productId, code);

            return Ok(new
            {
                Message = "ProductIdCode assigned successfully",
                GeneratedCode = code
            });
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
      
       

    }
}
