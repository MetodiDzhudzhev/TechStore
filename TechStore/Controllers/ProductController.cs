using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexByCategory(int categoryId)
        {
            IEnumerable<AllProductsByCategoryIdViewModel> allProducts = await this.productService.GetAllProductsByCategoryIdAsync(categoryId);

            return View(allProducts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                ProductDetailsViewModel? productDetails = await this.productService.GetProductDetailsViewModelAsync(id);

                if (productDetails == null)
                {
                    // TODO: Custom 404 page
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(productDetails);
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
