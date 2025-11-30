using InventoryMangmentMvc.Services;
using InventoryMangmentMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMVC.Controllers
{
    [Authorize(Roles = "InventoryEmployee")]
    public class InventoryEmployeeController : Controller
    {
        private readonly IApiService _apiService;

        public InventoryEmployeeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NearExpiration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NearExpiration(int days)
        {
            try
            {
                var products = await _apiService.GetAsync<List<ProductViewModel>>($"inventory-emp/near-expiration/{days}");
                return View("NearExpirationResult", products ?? new List<ProductViewModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View("NearExpiration");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllSends()
        {
            try
            {
                var sends = await _apiService.GetAsync<List<ProductSendViewModel>>("inventory-emp/all-sends");
                return View(sends ?? new List<ProductSendViewModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(new List<ProductSendViewModel>());
            }
        }

        [HttpGet]
        public IActionResult ReceiveProducts()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveProducts(int sendId)
        {
            try
            {
                var result = await _apiService.PostAsync<ProductSendViewModel>($"inventory-emp/receive-products/{sendId}", null);

                if (result == null)
                {
                    TempData["ErrorMessage"] = "Send not found";
                    return View();
                }

                return View("ReceiveProductsResult", result);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult SendToExternal()
        {
            return View(new List<ProductSendDetailViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> SendToExternal(List<ProductSendDetailViewModel> details)
        {
            if (details == null || !details.Any())
            {
                TempData["ErrorMessage"] = "No products to send";
                return View(details);
            }

            try
            {
                var result = await _apiService.PostAsync<ProductSendViewModel>("inventory-emp/send-to-external", details);

                if (result != null)
                    TempData["SuccessMessage"] = "Products sent successfully";
                else
                    TempData["ErrorMessage"] = "Failed to send products";

                return RedirectToAction("AllSends");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(details);
            }
        }


        [HttpGet]
        public IActionResult SendOrderToSupplier()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOrderToSupplier(CreateOrderViewModel model, string supplierId)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(model.OrderCode))
            {
                TempData["ErrorMessage"] = "Order Code is required";
                return View();
            }

            if (string.IsNullOrEmpty(model.OrderType))
            {
                TempData["ErrorMessage"] = "Order Type is required";
                return View();
            }

            if (model.ProductId <= 0)
            {
                TempData["ErrorMessage"] = "Product ID must be greater than 0";
                return View();
            }

            if (model.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than 0";
                return View();
            }

            if (string.IsNullOrEmpty(supplierId))
            {
                TempData["ErrorMessage"] = "Supplier ID is required";
                return View();
            }

            try
            {
                var result = await _apiService.PostAsync<object>(
                    $"inventory-emp/send-order-to-supplier?supplierId={supplierId}",
                    model
                );

                if (result != null)
                    TempData["SuccessMessage"] = "Order sent to supplier successfully";
                else
                    TempData["ErrorMessage"] = "Failed to send order";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }
    }
}
