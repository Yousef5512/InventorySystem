using InventoryMangmentMvc.Services;
using InventoryMangmentMvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentMvc.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IApiService _apiService;

        public SupplierController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReceiveFromInventory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveFromInventory(int productSendId)
        {
            var result = await _apiService.PostAsync<ProductSendViewModel>($"Supplier/receive-from-inventory/{productSendId}", null);

            if (result == null)
            {
                TempData["ErrorMessage"] = $"ProductSend {productSendId} not found";
                return View();
            }

            TempData["SuccessMessage"] = "Products received from inventory";
            return View("ReceiveResult", result);
        }

        [HttpPost]
        public async Task<IActionResult> SendToReceivingClerk(int productSendId)
        {
            var result = await _apiService.PostAsync<ProductSendViewModel>($"Supplier/send-to-receiving-clerk/{productSendId}", null);

            if (result == null)
            {
                TempData["ErrorMessage"] = $"ProductSend {productSendId} not found";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Products sent to receiving clerk";
            return RedirectToAction("Index");
        }
    }
}