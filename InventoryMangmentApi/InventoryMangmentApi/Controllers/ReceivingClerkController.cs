using Inventory.Bll.Contract;
using Inventory.Bll.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentApi.Controllers
{
    [ApiController]
    [Route("api/receiving-clerk")]
    [Authorize(Roles = "ReceivingClerk")]
    public class ReceivingClerkController : ControllerBase
    {
        private readonly IProductServices _productService;
        public ReceivingClerkController(IProductServices productService)
        {
            _productService = productService;
        }

       


    

        [HttpPost("receive-by-receiving-clerk/{productSendId}")]
        public async Task<IActionResult> ReceiveByReceivingClerk(int productSendId)
        {
            var sendDto = await _productService.ReceiveByReceivingClerkAsync(productSendId);

            if (sendDto == null)
                return NotFound($"ProductSend with ID {productSendId} not found.");

            return Ok(sendDto);
        }


        [HttpPost("UpdateInventoryChecked")]
        public async Task<IActionResult> UpdateInventoryChecked([FromBody] List<ReceivingCheckDto> receivedList)
        {
            await _productService.ReceiveProductsAsync(receivedList);
            return Ok("Products received successfully.");
        }



    }
}
