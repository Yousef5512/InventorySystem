using InventoryMangmentMvc.Services;
using InventoryMangmentMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentMvc.Controllers
{
    [Authorize(Roles = "ReceivingClerk")]
    public class ReceivingClerkController : Controller
    {
        private readonly IApiService _apiService;

        public ReceivingClerkController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReceiveProducts()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveProducts(int productSendId)
        {
            try
            {
                var result = await _apiService.PostAsync<ProductSendViewModel>(
                    $"receiving-clerk/receive-by-receiving-clerk/{productSendId}",
                    null
                );

                if (result == null)
                {
                    TempData["ErrorMessage"] = $"ProductSend with ID {productSendId} not found";
                    result = new ProductSendViewModel
                    {
                        Id = productSendId,
                        SendDate = DateTime.Now,
                        Status = "Not Found",
                        Details = new List<ProductSendDetailViewModel>()
                    };
                }
                else if (result.Details == null)
                {
                    result.Details = new List<ProductSendDetailViewModel>();
                }

                return View("ReceiveProductsResult", result);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInventoryChecked([FromForm] List<ReceivingCheckViewModel> receivedList)
        {
            if (receivedList == null || !receivedList.Any())
            {
                TempData["ErrorMessage"] = "No products selected for update.";
                return RedirectToAction("Index");
            }

            try
            {
                // Convert to API format if needed
                var apiData = receivedList.Select(r => new ReceivingCheckApiDto
                {
                    ProductId = r.ProductId,
                    Quantity = r.ReceivedQuantity,
                    IsDamaged = !r.IsReceived // If not received, mark as damaged
                }).ToList();

                var result = await _apiService.PostAsync<string>(
                    "receiving-clerk/UpdateInventoryChecked",
                    apiData
                );

                TempData["SuccessMessage"] = "Products received and inventory updated successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update inventory: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}