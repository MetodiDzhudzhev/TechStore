using Microsoft.AspNetCore.Authorization;
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
            try
            {
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
                Console.WriteLine(e.Message);
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
                    return NotFound();
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(int categoryId)
        {
            try
            {
                ProductFormInputModel inputModel = new ProductFormInputModel()
                {
                    Categories = await this.categoryService.GetCategoriesDropDownDataAsync(),
                    Brands = await this.brandService.GetBrandsDropDownDataAsync(),
                    CategoryId = categoryId,
                };

                return this.View(inputModel);
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(IndexByCategory));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    var deletedProduct = await this.productService
                        .GetDeletedProductByNameAsync(inputModel.Name);

                    if (deletedProduct != null)
                    {
                        return RedirectToAction(nameof(Restore), new { id = deletedProduct.Id });
                    }

                    else
                    {
                        ModelState.AddModelError(nameof(inputModel.Name), "Product with this name already exists.");
                        inputModel.Categories = await categoryService.GetCategoriesDropDownDataAsync();
                        inputModel.Brands = await brandService.GetBrandsDropDownDataAsync();
                        return View(inputModel);
                    }
                }

                bool result = await this.productService.AddProductAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Error occured while adding a product");
                    inputModel.Categories = await categoryService.GetCategoriesDropDownDataAsync();
                    inputModel.Brands = await brandService.GetBrandsDropDownDataAsync();
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

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Restore(string id)
        {
            ProductFormInputModel? product = await this.productService.GetProductForRestoreByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreConfirmed(string id)
        {
            bool result = await this.productService.RestoreByIdAsync(id);

            var product = await this.productService.GetProductByIdAsync(id);
            if (result == false)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                return RedirectToAction("IndexByCategory", new { categoryId = product.CategoryId });
            }

            // TODO: Implement it with the ILogger
            // TODO: Add JS bars
            return this.RedirectToAction("IndexByCategory", "Product", new { categoryId = product.CategoryId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(string? id)
        {
            try
            {
                ProductFormInputModel? model = await this.productService.GetEditableProductByIdAsync(this.GetUserId(), id);

                if (model == null)
                {
                    return NotFound();
                }

                model.Categories = await this.categoryService.GetCategoriesDropDownDataAsync();
                model.Brands = await this.brandService.GetBrandsDropDownDataAsync();

                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    inputModel.Categories = await this.categoryService.GetCategoriesDropDownDataAsync();
                    inputModel.Brands = await this.brandService.GetBrandsDropDownDataAsync();
                    ModelState.AddModelError(nameof(inputModel.Name), "Product with this name already exists.");
                    return View(inputModel);
                }

                bool result = await this.productService
                    .EditProductAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    this.ModelState.AddModelError(String.Empty, "Error occured while editing a product");
                    return this.View(inputModel);
                }

                return this.RedirectToAction(nameof(Details), new { id = inputModel.Id });

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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(string? id)
        {
            try
            {
                string userId = this.GetUserId();

                DeleteProductViewModel? modelForDelete = await this.productService
                    .GetProductForDeleteAsync(id, userId);

                if (modelForDelete == null)
                {
                    return NotFound();
                }

                return this.View(modelForDelete);
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(DeleteProductViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please do not modify the page");
                    return this.View(model);
                }

                bool deleteResult = await this.productService
                    .SoftDeleteProductAsync(this.GetUserId()!, model);

                if (deleteResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, "Error occured while deleting the product!");
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(IndexByCategory), new { categoryId = model.CategoryId });
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
