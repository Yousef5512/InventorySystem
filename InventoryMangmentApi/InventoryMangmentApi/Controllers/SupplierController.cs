using Inventory.Bll.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IProductServices productServices;

        public SupplierController(IProductServices productServices)
        {
            this.productServices = productServices;
        }

        // 1️⃣ Supplier receives list sent from Inventory Employee
        [HttpPost("receive-from-inventory/{productSendId}")]
        public async Task<IActionResult> ReceiveFromInventory(int productSendId)
        {
            var result = await productServices.ReceiveBySupplierAsync(productSendId);

            if (result == null)
                return NotFound($"ProductSend {productSendId} not found");

            return Ok(result);
        }

        // 2️⃣ Supplier sends list to Receiving Clerk
        [HttpPost("send-to-receiving-clerk/{productSendId}")]
        public async Task<IActionResult> SendToReceivingClerk(int productSendId)
        {
            var result = await productServices.SendToReceivingClerkAsync(productSendId);

            if (result == null)
                return NotFound($"ProductSend {productSendId} not found");

            return Ok(result);
        }
    }
}
