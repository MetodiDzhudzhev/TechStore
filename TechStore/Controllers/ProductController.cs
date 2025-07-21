using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IBrandService brandService;

        public ProductController(IProductService productService, ICategoryService categoryService,
            IBrandService brandService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexByCategory(int categoryId)
        {
            ProductsByCategoryViewModel? model = await productService.GetProductsByCategoryAsync(categoryId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> Add(int? categoryId)
        {
            try
            {
                ProductFormInputModel inputModel = new ProductFormInputModel()
                {
                    Categories = await this.categoryService.GetCategoriesDropDownDataAsync(),
                    Brands = await this.brandService.GetBrandsDropDownDataAsync(),
                    CategoryId = categoryId ?? 0,
                };

                return this.View(inputModel);
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                bool result = await this.productService.AddProductAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Error occured while adding a Product");
                    return this.View(inputModel);
                }

                return this.RedirectToAction("IndexByCategory", "Product", new { categoryId = inputModel.CategoryId });
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
