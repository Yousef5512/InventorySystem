using InventoryMangmentMvc.Services;
using InventoryMangmentMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentMvc.Controllers
{
    [Authorize(Roles = "PrimaryUser")]
    public class PrimaryController : Controller
    {
        private readonly IApiService _apiService;

        public PrimaryController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _apiService.GetAsync<List<ProductViewModel>>("primary");
                return View(products ?? new List<ProductViewModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                return View(new List<ProductViewModel>());
            }
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiService.PostAsync<ProductViewModel>("primary/create-product", model);

                if (result != null)
                {
                    TempData["SuccessMessage"] = "Product created successfully";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Failed to create product");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                var product = await _apiService.GetAsync<ProductViewModel>($"primary/{id}");

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found";
                    return RedirectToAction("Index");
                }

                var model = new ProductCreateViewModel
                {
                    ProductName = product.ProductName,
                    Quantity = product.Quantity,
                    Material = product.Material,
                    Size = product.Size.ToString(),
                    Type = product.Type,
                    Flavour = product.Flavour,
                    ExpirationDate = product.ExpirationDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Try PUT first, if API uses PUT for updates
                try
                {
                    var result = await _apiService.PutAsync<ProductViewModel>($"primary/{id}", model);
                    if (result != null)
                    {
                        TempData["SuccessMessage"] = "Product updated successfully";
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    // If PUT fails, try POST (some APIs use POST for updates)
                    var result = await _apiService.PostAsync<ProductViewModel>($"primary/{id}", model);
                    if (result != null)
                    {
                        TempData["SuccessMessage"] = "Product updated successfully";
                        return RedirectToAction("Index");
                    }
                }

                ModelState.AddModelError("", "Failed to update product");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                // Try DELETE method first
                try
                {
                    await _apiService.DeleteAsync($"primary/{id}");
                    TempData["SuccessMessage"] = "Product deleted successfully";
                }
                catch
                {
                    // If DELETE fails, try POST (some APIs use POST for delete)
                    await _apiService.PostAsync<object>($"primary/delete/{id}", null);
                    TempData["SuccessMessage"] = "Product deleted successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting product: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiService.PostAsync<object>("primary/assign-role", model);

                if (result != null)
                {
                    TempData["SuccessMessage"] = $"Role '{model.Role}' assigned to user '{model.Username}' successfully";
                    return RedirectToAction("AssignRole");
                }

                ModelState.AddModelError("", "Failed to assign role");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(List<ProductSendDetailViewModel> products)
        {
            if (products == null || !products.Any())
            {
                TempData["ErrorMessage"] = "No products selected";
                return View();
            }

            try
            {
                var result = await _apiService.PostAsync<ProductSendViewModel>("primary/create-order", products);

                if (result != null)
                {
                    TempData["SuccessMessage"] = "Order created successfully";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Failed to create order";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult DeleteUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Username is required";
                return View();
            }

            try
            {
                var result = await _apiService.PostAsync<string>($"primary/delete-user/{username}", null);
                TempData["SuccessMessage"] = $"User '{username}' deleted successfully";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }
    }
}
