using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelProductController : BaseControlPanelController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IBrandService brandService;

        private readonly ILogger<ControlPanelProductController> logger;

        private const int PageSize = 10;
        public ControlPanelProductController(IProductService productService, ICategoryService categoryService,
            IBrandService brandService, ILogger<ControlPanelProductController> logger)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.brandService = brandService;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(int? categoryId)
        {
            try
            {
                var categories = await this.categoryService.GetCategoriesDropDownDataAsync();

                int selectedCategoryId;

                if (categoryId.HasValue)
                {
                    if (categoryId <= 0)
                    {
                        logger.LogWarning("Negative categoryId supplied: {CategoryId}", categoryId);
                        return NotFound();
                    }

                    bool exists = await this.categoryService.ExistsAsync(categoryId.Value);

                    if (exists)
                    {
                        selectedCategoryId = categoryId.Value;
                    }
                    else
                    {
                        selectedCategoryId = categories.First().Id;
                    }
                }
                else
                {
                    selectedCategoryId = categories.First().Id;
                }

                ProductFormInputModel inputModel = new ProductFormInputModel()
                {
                    Categories = categories,
                    Brands = await this.brandService.GetBrandsDropDownDataAsync(),
                    CategoryId = selectedCategoryId,
                };

                return this.View(inputModel);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while preparing Add product form for category {CategoryId}!", categoryId);
                TempData["ErrorMessage"] = "Unable to prepare the Add product form. Please try again later.";
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = categoryId });
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
                    logger.LogWarning("Attempt by user {UserId} to add product with invalid model state", this.GetUserId());
                    return this.View(inputModel);
                }

                bool exist = await categoryService.ExistsAsync(inputModel.CategoryId);

                if (inputModel.CategoryId == 0 || exist == false)
                {
                    ModelState.AddModelError(nameof(inputModel.CategoryId), "Please select a valid category.");
                    inputModel.Categories = await categoryService.GetCategoriesDropDownDataAsync();
                    inputModel.Brands = await brandService.GetBrandsDropDownDataAsync();
                    return View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning("Attempt to add product name that already exists - {ProductName}", inputModel.Name);

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
                    logger.LogWarning("Failed to add product with name '{ProductName}' by user {UserId}", inputModel.Name, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Error occured while adding a product");
                    inputModel.Categories = await categoryService.GetCategoriesDropDownDataAsync();
                    inputModel.Brands = await brandService.GetBrandsDropDownDataAsync();
                    return this.View(inputModel);
                }

                logger.LogInformation("Product '{ProductName}' successfully added by user {UserId}!", inputModel.Name, this.GetUserId());
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = inputModel.CategoryId });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while adding product '{ProductName}'.", inputModel.Name);
                TempData["ErrorMessage"] = "An error occurred while adding the product. Please try again.";
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = inputModel.CategoryId });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Restore(string id)
        {
            ProductFormInputModel? product = await this.productService.GetProductForRestoreByIdAsync(id);

            if (product == null)
            {
                logger.LogWarning("Restore attempt for non-existing product with Id {ProductId} by user {UserId}", id, this.GetUserId());
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreConfirmed(string id)
        {
            try
            {
                bool result = await this.productService.RestoreByIdAsync(id);

                var product = await this.productService.GetProductByIdAsync(id);
                if (result == false)
                {
                    logger.LogError("Failed to restore product with Id {ProductId}.", id);
                    return this.RedirectToAction(nameof(Index), "Home");
                }

                logger.LogInformation("Product with Id {ProductId} was successfully restored by user {UserId}", id, this.GetUserId());
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = product!.CategoryId });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while restoring product with Id {ProductId}!", id);
                TempData["ErrorMessage"] = "We couldn't restore the product. Please try again later.";
                return this.RedirectToAction(nameof(Index), "Home");
            }
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
                    logger.LogWarning("Product with Id {ProductId} was not found", id);
                    return NotFound();
                }

                model.Categories = await this.categoryService.GetCategoriesDropDownDataAsync();
                model.Brands = await this.brandService.GetBrandsDropDownDataAsync();

                return this.View(model);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while preparing Edit product form for product with Id {ProductId}!", id);
                TempData["ErrorMessage"] = "An error occurred while preparing the Edit product form.";
                return this.RedirectToAction(nameof(Index), "Home");
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
                    logger.LogWarning("Attempt to edit product with Id {ProductId} with invalid model state by user {UserId}", inputModel.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Error occured while editing the product. Please review the details and try again.");
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
                    logger.LogWarning("Failed to edit product '{ProductName}'!", inputModel.Name);
                    this.ModelState.AddModelError(String.Empty, "Error occured while editing the product!");
                    return this.View(inputModel);
                }

                logger.LogInformation("Product '{ProductName}' successfully edited by user {UserId}", inputModel.Name, this.GetUserId());
                return this.RedirectToAction("Details", "Product", new { area = "", id = inputModel.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while editing product '{ProductName}'!", inputModel.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while editing the product. Please try again.";
                return this.RedirectToAction(nameof(Index), "Home");
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
                    logger.LogWarning("Product with Id {ProductId} was not found", id);
                    return NotFound();
                }

                return this.View(modelForDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while preparing Delete product form for product with Id {ProductId}!", id);
                TempData["ErrorMessage"] = "An error occurred while preparing the Delete product page.";
                return this.RedirectToAction(nameof(Index), "Home");
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
                    logger.LogWarning("Attempt to delete product with Id {ProductId} with invalid model state by user {UserId}", model.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Please do not modify the page");
                    return this.View(model);
                }

                bool deleteResult = await this.productService
                    .SoftDeleteProductAsync(this.GetUserId()!, model);

                if (deleteResult == false)
                {
                    logger.LogWarning("Failed to delete product with Id {ProductId} by user {UserId}!", model.Id, this.GetUserId());
                    this.ModelState.AddModelError(string.Empty, "Error occured while deleting the product!");
                    return this.View(model);
                }

                logger.LogInformation("Product {ProductId} successfully deleted by user {UserId}", model.Id, this.GetUserId());
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = model.CategoryId });

            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while deleting product with Id {ProductId}", model.Id);
                TempData["ErrorMessage"] = "An error occurred while deleting the product. Please try again.";
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            int totalCount = await productService.GetTotalCountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            IEnumerable<ProductManageViewModel> products = await productService.GetPagedAsync(page, PageSize);

            ProductManageListViewModel viewModel = new ProductManageListViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
    }
}
