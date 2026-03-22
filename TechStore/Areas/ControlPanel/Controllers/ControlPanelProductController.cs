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

                ProductFormInputModel inputModel = new ProductFormInputModel()
                {
                    Categories = categories,
                    Brands = await this.brandService.GetBrandsDropDownDataAsync(),
                };

                return this.View(inputModel);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.AddProductPageLoadError);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.AddProductPageLoadError;
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = categoryId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ProductLog.AddWithInvalidModelState, this.GetUserId());
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                bool exist = await categoryService.ExistsAsync(inputModel.CategoryId);

                if (inputModel.CategoryId == 0 || exist == false)
                {
                    logger.LogWarning(ProductLog.CategoryNotValid);
                    ModelState.AddModelError(nameof(inputModel.CategoryId), ProductUi.CategoryNotValid);
                    await PopulateDropdowns(inputModel);
                    return View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
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

                bool result = await this.productService.AddProductAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning(ProductLog.AddFailed, inputModel.Name, this.GetUserId());
                    ModelState.AddModelError(string.Empty, ProductUi.AddFailed);
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                logger.LogInformation(ProductLog.AddSuccess, inputModel.Name, this.GetUserId());
                TempData[TempDataKeys.SuccessMessage] = ProductUi.AddSuccess;
                return this.RedirectToAction("IndexByCategory", "Product", new { area = "", categoryId = inputModel.CategoryId });
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.AddError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.AddError;
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
                logger.LogWarning(ProductLog.RestoreNonExistingProduct, id, this.GetUserId());
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreConfirmed(string id)
        {
            try
            {
                bool result = await this.productService.RestoreByIdAsync(id);

                if (result == false)
                {
                    logger.LogWarning(ProductLog.RestoreFailed, id);
                    TempData[TempDataKeys.ErrorMessage] = ProductUi.RestoreFailed;
                    return this.RedirectToAction(nameof(Index), "Home");
                }

                logger.LogInformation(ProductLog.RestoreSuccess, id, this.GetUserId());
                TempData[TempDataKeys.SuccessMessage] = ProductUi.RestoreSuccess;

                return this.RedirectToAction("Manage", "ControlPanelProduct", new { area = "ControlPanel"});
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.RestoreError, id);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.RestoreError;
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
                    logger.LogWarning(ProductLog.NotFound, id);
                    return NotFound();
                }

                await PopulateDropdowns(model);

                return this.View(model);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.EditProductPageLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.EditProductPageLoadError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(ProductFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ProductLog.EditWithInvalidModelState, inputModel.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, ProductUi.EditWithInvalidModelState);
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                if (await productService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    await PopulateDropdowns(inputModel);
                    ModelState.AddModelError(nameof(inputModel.Name), ProductUi.NameAlreadyExist);
                    return View(inputModel);
                }

                bool result = await this.productService
                    .EditProductAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning(ProductLog.EditFailed, inputModel.Name);
                    this.ModelState.AddModelError(String.Empty, ProductUi.EditFailed);
                    await PopulateDropdowns(inputModel);
                    return this.View(inputModel);
                }

                logger.LogInformation(ProductLog.EditSuccess, inputModel.Name, this.GetUserId());
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
                    logger.LogWarning(ProductLog.NotFound, id);
                    return NotFound();
                }

                return this.View(modelForDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, ProductLog.DeleteProductPageLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = ProductUi.DeleteProductPageLoadError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(DeleteProductViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ProductLog.DeleteWithInvalidModelState, model.Id, this.GetUserId());
                    return this.View(model);
                }

                bool deleteResult = await this.productService
                    .SoftDeleteProductAsync(this.GetUserId()!, model);

                if (deleteResult == false)
                {
                    logger.LogWarning(ProductLog.DeleteFailed, model.Id, this.GetUserId());
                    this.ModelState.AddModelError(string.Empty, ProductUi.DeleteFailed);
                    return this.View(model);
                }

                logger.LogInformation(ProductLog.DeleteSuccess, model.Id, this.GetUserId());
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
        [Authorize(Roles = "Admin,Manager")]
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
    }
}