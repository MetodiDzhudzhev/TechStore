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
    }
}
