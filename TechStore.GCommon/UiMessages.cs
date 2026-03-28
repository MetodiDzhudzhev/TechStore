namespace TechStore.GCommon
{
    public static class UiMessages
    {
        public static class Brand
        {
            public const string InvalidId = "Invalid Id.";
            public const string DetailsLoadError = "An error occurred while loading the brand details. Please try again later.";
            public const string NameAlreadyExist = "Brand with this name already exists.";
            public const string AddError = "Error occurred while adding the brand.";
            public const string RestoreSuccess = "Brand restored successfully!";
            public const string RestoreFailed = "Failed to restore the brand!";
            public const string RestoreError = "We couldn't restore the brand. Please try again later.";
            public const string EditBrandPageLoadError = "An error occurred while preparing the Edit brand form.";
            public const string EditWithInvalidModelState = "Error occurred while editing the brand. Please review the details and try again.";
            public const string EditError = "An unexpected error occurred while editing the brand. Please try again.";
            public const string DeleteBrandPageLoadError = "An error occurred while preparing the Delete brand form.";
            public const string DeleteFailed = "Failed to delete the brand!";
            public const string DeleteError = "An unexpected error occurred while deleting the brand. Please try again.";
        }

        public static class Cart
        {
            public const string ProductAddError = "An error occurred while adding the product. Please try again.";
            public const string IncreaseError = "An error occurred while increasing the quantity of the product. Please try again.";
            public const string RemoveError = "An error occurred while removing the product. Please try again.";
            public const string DecreaseError = "An error occurred while decreasing the product. Please try again.";
            public const string ClearError = "An error occurred while cleaning the cart. Please try again.";
        }

        public static class Order
        {
            public const string NotFound = "Order was not found";
            public const string CheckoutError = "An unexpected error occurred. Please try again later.";
            public const string CreateFailed = "Failed to create order. Please check product availability.";
            public const string EditOrderPageLoadError = "An error occurred while preparing the Edit order form.";
            public const string EditStatusFailed = "Invalid status transition or locked order.";
            public const string EditStatusSuccess = "Order status updated successfully.";
            public const string EditStatusError = "An unexpected error occurred while changing the order status.";
            public const string EditShippingDetailsFailed = "Shipping details cannot be updated. The order may be locked.";
            public const string EditShippingDetailsSuccess = "Shipping details updated successfully.";
            public const string EditShippingDetailsError = "An unexpected error occurred while updating shipping details.";
        }

        public static class Payment
        {
            public const string InitializationFailed = "Payment initialization failed. Please try again.";
            public const string CancelPageLoadError = "An unexpected error occurred while loading cancel payment page.";
        }

        public static class Product
        {
            public const string LoadProductsError = "An unexpected error occurred while loading products from this category.";
            public const string DetailsLoadError = "An error occurred while loading the product details. Please try again later.";
            public const string EmptySearchQuery = "Please enter a keyword to search.";
            public const string SearchError = "An unexpected error occurred. Please try again later.";
            public const string AddProductPageLoadError = "Unable to prepare the Add product form. Please try again later.";
            public const string CategoryNotValid = "Please select a valid category.";
            public const string BrandNotValid = "Please select a valid brand.";
            public const string NameAlreadyExist = "Product with this name already exists.";
            public const string AddFailed = "Failed to add the product";
            public const string AddSuccess = "The product was added successfully!";
            public const string AddError = "An error occurred while adding the product. Please try again.";
            public const string RestoreFailed = "We couldn't restore the product. Please try again later.";
            public const string RestoreSuccess = "The product was restored successfully!";
            public const string RestoreError = "An unexpected error occurred while restoring the product. Please try again later!";
            public const string EditProductPageLoadError = "An error occurred while preparing the Edit product form.";
            public const string EditWithInvalidModelState = "Error occurred while editing the product. Please review the details and try again.";
            public const string EditFailed = "Failed to edit the product!";
            public const string EditError = "An unexpected error occurred while editing the product. Please try again.";
            public const string DeleteProductPageLoadError = "An error occurred while preparing the Delete product page.";
            public const string DeleteFailed = "Failed to delete the product!";
            public const string DeleteSuccess = "The product was deleted successfully!";
            public const string DeleteError = "An error occurred while deleting the product. Please try again.";
        }

        public static class Review
        {
            public const string InvalidInput = "Please select a rating (1-5) and try again.";
            public const string AlreadyAdded = "You already have a review for this product.";
            public const string AddSuccess = "Your review was added successfully!";
            public const string AddError = "An error occurred while adding review. Please try again.";
            public const string DeleteFailed = "Review #{0} not found or already deleted.";
            public const string DeleteSuccess = "Review #{0} was successfully deleted.";
            public const string DeleteError = "An error occurred while deleting review #{0}. Please try again.";
        }

        public static class Account
        {
            public const string DeliveryDetailsUpdateError = "Unable to update delivery details. Please try again.";
            public const string DeliveryDetailsUpdateSuccess = "Delivery details updated successfully.";
        }

        public static class Category
        {
            public const string InvalidId = "Invalid Id.";
            public const string NameAlreadyExist = "Category with this name already exists.";
            public const string AddError = "Error occurred while adding the category.";
            public const string RestoreSuccess = "Category restored successfully!";
            public const string RestoreFailed = "Failed to restore the category!";
            public const string RestoreError = "We couldn't restore the category. Please try again later.";
            public const string EditCategoryPageLoadError = "An error occurred while preparing the Edit category form.";
            public const string EditWithInvalidModelState = "Error occurred while editing the category. Please review the details and try again.";
            public const string EditError = "An unexpected error occurred while editing the category. Please try again.";
            public const string DeleteCategoryPageLoadError = "An error occurred while preparing the Delete category form.";
            public const string DeleteFailed = "Failed to delete the category!";
            public const string DeleteError = "An error occurred while deleting the category. Please try again.";
        }

        public static class User
        {
            public const string RoleNotSelected = "Please select a role.";
            public const string RoleAssignSuccess = "Role assigned successfully.";
            public const string RoleAssignFailed = "Failed to assign role.";
            public const string RoleAssignError = "An error occurred while assigning the role. Please try again.";
        }
    }
}
