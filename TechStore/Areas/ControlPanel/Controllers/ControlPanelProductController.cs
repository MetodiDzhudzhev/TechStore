using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;

using TechStore.GCommon;
using ProductLog = TechStore.GCommon.LogMessages.Product;
using ProductUi = TechStore.GCommon.UiMessages.Product;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    [Authorize(Roles = "Admin,Manager")]
    public class ControlPanelProductController : BaseControlPanelController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IBrandService brandService;

        private readonly ILogger<ControlPanelProductController> logger;
        private Guid UserId => Guid.Parse(this.GetUserId()!);

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
        public async Task<IActionResult> Add(int? categoryId)
        {
            try
            {
                ProductFormInputModel inputModel = new ProductFormInputModel()
                {
                    Categories = await this.categoryService.GetCategoriesDropDownDataAsync(),
                    Brands = await this.brandService.GetBrandsDropDownDataAsync(),
                };

                return this.View(inputModel);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.AddProductPageLoadError);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.AddProductPageLoadError;
                return RedirectToProductCategory(categoryId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ProductLog.AddWithInvalidModelState, this.UserId);
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                bool categoryExist = await categoryService.ExistsAsync(inputModel.CategoryId);

                if (categoryExist == false)
                {
                    logger.LogWarning(ProductLog.CategoryNotValid);
                    ModelState.AddModelError(nameof(inputModel.CategoryId), ProductUi.CategoryNotValid);
                    await PopulateDropdowns(inputModel);
                    return View(inputModel);
                }

                bool brandExist = await brandService.ExistsAsync(inputModel.BrandId);

                if (brandExist == false)
                {
                    logger.LogWarning(ProductLog.BrandNotValid);
                    ModelState.AddModelError(nameof(inputModel.BrandId), ProductUi.BrandNotValid);
                    await PopulateDropdowns(inputModel);
                    return View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id.ToString()))
                {
                    logger.LogWarning(ProductLog.NameAlreadyExist, inputModel.Name);

                    var deletedProduct = await this.productService
                        .GetDeletedProductByNameAsync(inputModel.Name);

                    if (deletedProduct != null)
                    {
                        return RedirectToAction(nameof(Restore), new { id = deletedProduct.Id });
                    }

                    else
                    {
                        ModelState.AddModelError(nameof(inputModel.Name), ProductUi.NameAlreadyExist);
                        await PopulateDropdowns(inputModel);
                        return View(inputModel);
                    }
                }

                await this.productService.AddProductAsync(inputModel);

                logger.LogInformation(ProductLog.AddSuccess, inputModel.Name, this.UserId);
                TempData[TempDataKeys.SuccessMessage] = ProductUi.AddSuccess;
                return RedirectToProductCategory(inputModel.CategoryId);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.AddError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.AddError;
                return RedirectToProductCategory(inputModel.CategoryId);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Restore(string id)
        {
            if (!TryParseId(id, out Guid productId))
            {
                return NotFound();
            }

            ProductFormInputModel? product = await this.productService.GetProductForRestoreByIdAsync(productId);

            if (product == null)
            {
                logger.LogWarning(ProductLog.RestoreNonExistingProduct, productId, this.UserId);
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(string id)
        {
            if (!TryParseId(id, out Guid productId))
            {
                return NotFound();
            }

            try
            {
                bool result = await this.productService.RestoreByIdAsync(productId);

                if (result == false)
                {
                    logger.LogWarning(ProductLog.RestoreFailed, productId);
                    TempData[TempDataKeys.ErrorMessage] = ProductUi.RestoreFailed;
                    return this.RedirectToAction(nameof(Index), "Home");
                }

                logger.LogInformation(ProductLog.RestoreSuccess, productId, this.UserId);
                TempData[TempDataKeys.SuccessMessage] = ProductUi.RestoreSuccess;

                return this.RedirectToAction("Manage", "ControlPanelProduct", new { area = "ControlPanel"});
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.RestoreError, productId);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.RestoreError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (!TryParseId(id, out Guid productId))
            {
                return NotFound();
            }

            try
            {
                ProductFormInputModel? model = await this.productService.GetEditableProductByIdAsync(productId);

                if (model == null)
                {
                    logger.LogWarning(ProductLog.NotFound, productId);
                    return NotFound();
                }

                await PopulateDropdowns(model);

                return this.View(model);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.EditProductPageLoadError, productId);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.EditProductPageLoadError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ProductLog.EditWithInvalidModelState, inputModel.Id, this.UserId);
                    ModelState.AddModelError(string.Empty, ProductUi.EditWithInvalidModelState);
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                bool categoryExist = await categoryService.ExistsAsync(inputModel.CategoryId);

                if (categoryExist == false)
                {
                    logger.LogWarning(ProductLog.CategoryNotValid);
                    ModelState.AddModelError(nameof(inputModel.CategoryId), ProductUi.CategoryNotValid);
                    await PopulateDropdowns(inputModel);
                    return View(inputModel);
                }

                bool brandExist = await brandService.ExistsAsync(inputModel.BrandId);

                if (brandExist == false)
                {
                    logger.LogWarning(ProductLog.BrandNotValid);
                    ModelState.AddModelError(nameof(inputModel.BrandId), ProductUi.BrandNotValid);
                    await PopulateDropdowns(inputModel);
                    return View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id.ToString()))
                {
                    await PopulateDropdowns(inputModel);
                    ModelState.AddModelError(nameof(inputModel.Name), ProductUi.NameAlreadyExist);
                    return View(inputModel);
                }

                bool result = await this.productService.EditProductAsync(inputModel);

                if (result == false)
                {
                    logger.LogWarning(ProductLog.EditFailed, inputModel.Name);
                    this.ModelState.AddModelError(String.Empty, ProductUi.EditFailed);
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                logger.LogInformation(ProductLog.EditSuccess, inputModel.Name, this.UserId);
                return this.RedirectToAction("Details", "Product", new { area = "", id = inputModel.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.EditError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.EditError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (!TryParseId(id, out Guid productId))
            {
                return NotFound();
            }

            try
            {
                DeleteProductViewModel? modelForDelete = await this.productService.GetProductForDeleteAsync(productId);

                if (modelForDelete == null)
                {
                    logger.LogWarning(ProductLog.NotFound, productId);
                    return NotFound();
                }

                return this.View(modelForDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.DeleteProductPageLoadError, productId);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.DeleteProductPageLoadError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteProductViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ProductLog.DeleteWithInvalidModelState, model.Id, this.UserId);
                    return this.View(model);
                }

                bool deleteResult = await this.productService.SoftDeleteProductAsync(model);

                if (deleteResult == false)
                {
                    logger.LogWarning(ProductLog.DeleteFailed, model.Id, this.UserId);
                    this.ModelState.AddModelError(string.Empty, ProductUi.DeleteFailed);
                    return this.View(model);
                }

                logger.LogInformation(ProductLog.DeleteSuccess, model.Id, this.UserId);
                TempData[TempDataKeys.SuccessMessage] = ProductUi.DeleteSuccess;
                return this.RedirectToAction("Manage", "ControlPanelProduct", new { area = "ControlPanel" });

            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.DeleteError, model.Id);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.DeleteError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int page = 1)
        {
            ProductManageListViewModel viewModel = await productService.GetPagedAsync(page, PageSize);

            return View(viewModel);
        }
        private async Task PopulateDropdowns(ProductFormInputModel model)
        {
            model.Categories = await categoryService.GetCategoriesDropDownDataAsync();
            model.Brands = await brandService.GetBrandsDropDownDataAsync();
        }
        private IActionResult RedirectToProductCategory(int? categoryId)
        {
            return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId });
        }
        private bool TryParseId(string? id, out Guid parsedId)
        {
            return Guid.TryParse(id, out parsedId);
        }
    }
}