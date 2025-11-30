using InventoryMangmentMvc.Services;
using InventoryMangmentMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMVC.Controllers
{
    [Authorize(Roles = "InventoryOrganizer")]
    public class InventoryOrganizerController : Controller
    {
        private readonly IApiService _apiService;

        public InventoryOrganizerController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _apiService.GetAsync<List<ProductViewModel>>("inventory-organizer/products");
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AssignProductId(int productId)
        {
            var result = await _apiService.PostAsync<object>($"inventory-organizer/assign-product-id/{productId}", null);

            if (result != null)
                TempData["SuccessMessage"] = "Product ID assigned successfully";
            else
                TempData["ErrorMessage"] = "Failed to assign product ID";

            return RedirectToAction("Index");
        }
    }
}