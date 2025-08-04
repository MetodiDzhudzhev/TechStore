using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;
using static TechStore.GCommon.ApplicationConstants;


namespace TechStore.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        private readonly ILogger<ProductController> logger;

        public ProductController(IProductService productService, ICategoryService categoryService,
            ILogger<ProductController> logger)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> IndexByCategory(int categoryId)
        {
            try
            {
                bool categoryExists = await this.categoryService.ExistsAsync(categoryId);
                if (!categoryExists)
                {
                    logger.LogWarning("Attempt to access IndexByCategory with non-existing categoryId - {CategoryId}", categoryId);
                    return NotFound();
                }
                ProductsByCategoryViewModel? products = await productService
                .GetProductsByCategoryAsync(categoryId);

                if (products == null)
                {
                    return NotFound();
                }

                return View(products);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in IndexByCategory with categoryId {CategoryId}", categoryId);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading products from this category.";
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                ProductDetailsViewModel? productDetails = await this.productService
                    .GetProductDetailsViewModelAsync(id);

                if (productDetails == null)
                {
                    logger.LogWarning("Product with ID {ProductId} was not found", id);
                    return NotFound();
                }

                return this.View(productDetails);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while loading details for product with Id {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the product details. Please try again later.";
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.Message = "Please enter a keyword to search.";
                return View(Enumerable.Empty<ProductInCategoryViewModel>());
            }

            IEnumerable<ProductInCategoryViewModel?> products = await productService.SearchByKeywordAsync(query);

            ViewBag.Query = query;
            return View(products);
        }
    }
}
