using Inventory.Bll.Contract;
using Inventory.Bll.DTOs;
using Inventory.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentApi.Controllers
{
    [ApiController]
    [Route("api/inventory-emp")]
    [Authorize(Roles = "InventoryEmployee")]
    public class InventoryEmployeeController : ControllerBase
    {
        private readonly IProductServices _productService;
        private readonly IOrderServices _orderService;

        public InventoryEmployeeController(IProductServices productService, IOrderServices orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }

    

        [HttpGet("near-expiration/{days}")]
        public async Task<IActionResult> NearExpiration(int days)
        {
            var list = await _productService.GetNearExpirationAsync(days);
            return Ok(list);
        }

        
        [HttpPost("receive-products/{sendId}")]
        public async Task<IActionResult> ReceiveProducts(int sendId)
        {
            var send = await _productService.GetProductSendAsync(sendId);

            if (send == null) return NotFound("Send not found");

            foreach (var detail in send.Details)
            {
                var available = await _orderService.CheckAvailabilityAsync(detail.ProductId, detail.Quantity);
                detail.Available = available;
                detail.Received = available;
            }

            send.Status = send.Details.All(d => d.Received) ? "Received" : "Partial";
            await _productService.UpdateProductSendAsync(send);

            // Map to DTO
            var sendDto = new ProductSendDto
            {
                Id = send.Id,
                SendDate = send.SendDate,
                Status = send.Status,
                Details = send.Details.Select(d => new ProductSendDetailDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product.ProductName,
                    Quantity = d.Quantity,
                    Available = d.Available,
                    Received = d.Received
                }).ToList()
            };

            return Ok(sendDto);
        }



        [HttpPost("send-to-external")]
        public async Task<IActionResult> SendToExternal([FromBody] List<ProductSendDetailDto> details)
        {
            if (details == null || details.Count == 0)
                return BadRequest("No products to send.");

            var result = await _productService.SendToExternalAsync(details);
            return Ok(result);
        }

        [HttpGet("all-sends")]
        public async Task<IActionResult> GetAllSends()
        {
            var result = await _productService.GetAllSendsAsync();
            return Ok(result);
        }

        [HttpPost("send-order-to-supplier")]
        public async Task<IActionResult> SendOrderToSupplier(CreateOrderDto dto, string supplierId)
        {
            var result = await _productService.SendOrderToSupplierAsync(dto, supplierId);
            return Ok(result);
        }

    }
}
