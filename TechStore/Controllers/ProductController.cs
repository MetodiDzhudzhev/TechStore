using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;

using TechStore.GCommon;
using ProductLog = TechStore.GCommon.LogMessages.Product;
using ProductUi = TechStore.GCommon.UiMessages.Product;

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
        public async Task<IActionResult> IndexByCategory(int categoryId, ProductSort sort = ProductSort.NameAsc)
        {
            try
            {
                ProductsByCategoryViewModel? products = await productService
                .GetProductsByCategoryAsync(categoryId, sort);

                if (products == null)
                {
                    logger.LogWarning(ProductLog.CategoryNotFound, categoryId);
                    return NotFound();
                }

                return View(products);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.CategoryProductsLoadError, categoryId);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.LoadProductsError;
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
                    logger.LogWarning(ProductLog.NotFound, id);
                    return NotFound();
                }

                return this.View(productDetails);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.DetailsLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.DetailsLoadError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.Message = ProductUi.EmptySearchQuery;
                logger.LogInformation(ProductLog.EmptySearchQueryAttempt);
                return View(Enumerable.Empty<ProductInCategoryViewModel>());
            }

            try
            {
                IEnumerable<ProductInCategoryViewModel?> products = await productService.SearchByKeywordAsync(query);
                ViewBag.Query = query;
                return View(products);
            }
            catch (Exception e  )
            {
                logger.LogError(e, ProductLog.SearchError, query);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.SearchError;
                return RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
