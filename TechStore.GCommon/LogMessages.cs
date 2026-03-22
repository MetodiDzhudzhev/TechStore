namespace TechStore.GCommon
{
    public static class LogMessages
    {
        public static class Brand
        {
            public const string NotFound = "Brand with Id {BrandId} was not found.";
            public const string DetailsLoadError = "An error occurred while loading details for brand with Id {BrandId}.";
            public const string AddWithInvalidModelState = "Attempt by user {UserId} to add brand with invalid model state.";
            public const string NameAlreadyExist = "Attempt to add brand name that already exists - {BrandName}.";
            public const string AddFailed = "Failed to add brand with name '{BrandName}' by user {UserId}.";
            public const string AddSuccess = "Brand '{BrandName}' successfully added by user {UserId}.";
            public const string AddError = "Exception occurred while adding brand '{BrandName}'.";
            public const string RestoreInvalidBrand = "Restore attempt for non-existing brand with Id {BrandId} by user {UserId}.";
            public const string RestoreFailed = "Failed to restore brand with Id {BrandId}.";
            public const string RestoreSuccess = "Brand with Id {BrandId} was successfully restored by user {UserId}.";
            public const string RestoreError = "Exception occurred while restoring brand with Id {BrandId}.";
            public const string EditBrandPageLoadError = "Error while preparing Edit brand form for brand with Id {BrandId}.";
            public const string EditWithInvalidModelState = "Attempt to edit brand with Id {BrandId} with invalid model state by user {UserId}.";
            public const string EditFailed = "Failed to edit brand '{BrandName}'.";
            public const string EditSuccess = "Brand '{BrandName}' successfully edited by user {UserId}.";
            public const string EditError = "Exception occurred while editing brand '{BrandName}'.";
            public const string DeleteBrandPageLoadError = "Exception while preparing Delete brand form for brand with Id {BrandId}.";
            public const string DeleteWithInvalidModelState = "Attempt to delete brand with Id {BrandId} with invalid model state by user {UserId}.";
            public const string DeleteFailed = "Failed to delete brand with Id {BrandId} by user {UserId}.";
            public const string DeleteSuccess = "Brand with Id {BrandId} successfully deleted by user {UserId}.";
            public const string DeleteError = "Exception occurred while deleting brand with Id {BrandId}.";
        }

        public static class Cart
        {
            public const string ProductAddFailed = "Failed to add product with id {ProductId} to cart of user with id {UserId}.";
            public const string ProductAdded = "Product with id {ProductId} successfully added to the cart of user with id {UserId}.";
            public const string ProductAddError = "Exception occurred while adding product {ProductId}.";
            public const string ProductIncreaseFailed = "Failed to increase the quantity of product with id {ProductId} in the cart of user with id {UserId}.";
            public const string ProductIncreaseSuccess = "Quantity of Product with id {ProductId} in the cart of user with id {UserId} successfully increased by one.";
            public const string ProductIncreaseError = "Exception occurred while increasing the quantity of product with id {ProductId}.";
            public const string ProductRemoveFailed = "Failed to remove product with id {ProductId} from cart of user with id {UserId}.";
            public const string ProductRemoved = "Product with id {ProductId} successfully removed from the cart of user with id {UserId}.";
            public const string ProductRemoveError = "Exception occurred while removing product with id {ProductId}.";
            public const string ProductDecreaseFailed = "Failed to decrease product with id {ProductId} from cart of user with id {UserId}.";
            public const string ProductDecreased = "Product with id {ProductId} was successfully decreased by one from the cart of user with id {UserId}.";
            public const string ProductDecreaseError = "Exception occurred while decreasing product with id{ProductId}.";
            public const string CartClearFailed = "Failed to clear the cart of user with id {UserId}.";
            public const string CartCleared = "The cart of user with id {UserId} has ben cleared succesfully.";
            public const string CartClearError = "Exception occurred while cleaning the cart of user with id {UserId}.";
        }

        public static class Home
        {
            public const string ErrorStatusCode = "Error occurred with status code: {statusCode}";
        }

        public static class Order
        {
            public const string NotFound = "Order with Id {OrderId} was not found.";
            public const string InvalidCheckoutForm = "Checkout form submitted by user {UserId} is invalid.";
            public const string CreateFailed = "User {UserId} failed to create an order.";
            public const string Create = "User {UserId} successfully created order {OrderId}.";
            public const string CheckoutError = "An unexpected error occurred while user {UserId} tried to checkout.";
            public const string EditOrderPageLoadError = "Error while preparing Edit order form for order with Id {OrderId}.";
            public const string EditStatusFailed = "Failed to update status for order with Id {OrderId}.";
            public const string EditStatusSuccess = "Status of order with Id {OrderId} was updated successfully.";
            public const string EditStatusError = "Exception occurred while changing status for order with Id {OrderId}.";
            public const string EditShippingDetailsFailed = "Failed to update shipping details for order with Id {OrderId}.";
            public const string EditShippingDetailsSuccess = "Shipping details for order with Id {OrderId} updated successfully.";
            public const string EditShippingDetailsError = "Exception occurred while editing shipping details for order with Id {OrderId}.";
        }

        public static class Payment
        {
            public const string PaymentPageLoadError = "Unexpected error while loading payment page. OrderId: {OrderId}";
            public const string PaymentSummaryNotFound = "Payment summary not found. OrderId={OrderId}, UserId={UserId}";
            public const string OrderProductsNotFound = "No products found for order. OrderId={OrderId}, UserId={UserId}";
            public const string PaymentSuccessDataMissing = "Payment success data not found. OrderId={OrderId}, UserId={UserId}";
            public const string StripeSessionLookupFailed = "Stripe session lookup failed. SessionId={SessionId}";
            public const string StripeSessionCreationFailed = "Stripe session creation failed. OrderId={OrderId}";
            public const string MissingOrderIdMetadata = "Stripe session missing orderId metadata. SessionId={SessionId}";
            public const string UnauthenticatedPaymentAccess = "Unauthenticated user attempted to access payment functionality.";
        }

        public static class Product
        {
            public const string CategoryNotFound = "Attempt to access IndexByCategory with non-existing categoryId - {CategoryId}";
            public const string CategoryProductsLoadError = "Error in IndexByCategory with categoryId {CategoryId}";
            public const string NotFound = "Product with ID {ProductId} was not found.";
            public const string DetailsLoadError = "An error occurred while loading details for product with Id {ProductId}";
            public const string EmptySearchQueryAttempt = "Empty search query submitted.";
            public const string SearchError = "Error occurred while searching products. Query={Query}";
            public const string NameAlreadyExist = "Attempt to add product name that already exists - {ProductName}";
            public const string CategoryNotValid = "Invalid category selected.";
            public const string AddProductPageLoadError = "Error while preparing Add product form.";
            public const string AddWithInvalidModelState = "Attempt by user {UserId} to add product with invalid model state.";
            public const string AddFailed = "Failed to add product with name '{ProductName}' by user {UserId}";
            public const string AddSuccess = "Product '{ProductName}' successfully added by user {UserId}";
            public const string AddError = "Exception occurred while adding product '{ProductName}'.";
            public const string RestoreNonExistingProduct = "Restore attempt for non-existing product with Id {ProductId} by user {UserId}";
            public const string RestoreFailed = "Failed to restore product with Id {ProductId}.";
            public const string RestoreSuccess = "Product with Id {ProductId} was successfully restored by user {UserId}";
            public const string RestoreError = "Exception occurred while restoring product with Id {ProductId}";
            public const string EditProductPageLoadError = "Error while preparing Edit product form for product with Id {ProductId}!";
            public const string EditWithInvalidModelState = "Attempt to edit product with Id {ProductId} with invalid model state by user {UserId}";
            public const string EditFailed = "Failed to edit product '{ProductName}'";
            public const string EditSuccess = "Product '{ProductName}' successfully edited by user {UserId}";
            public const string EditError = "Exception occurred while editing product '{ProductName}'";
            public const string DeleteProductPageLoadError = "Exception while preparing Delete product form for product with Id {ProductId}";
            public const string DeleteWithInvalidModelState = "Attempt to delete product with Id {ProductId} with invalid model state by user {UserId}";
            public const string DeleteFailed = "Failed to delete product with Id {ProductId} by user {UserId}";
            public const string DeleteSuccess = "Product {ProductId} successfully deleted by user {UserId}";
            public const string DeleteError = "Exception occurred while deleting product with Id {ProductId}";
        }

        public static class Review
        {
            public const string InvalidModelState = "Attempt by user {UserId} to add review with invalid model state.";
            public const string AddFailed = "User {UserId} failed to add review to product {ProductId}";
            public const string AddSuccess = "User {UserId} successfully added review to product {ProductId}";
            public const string AddError = "Exception occurred while adding review to product '{ProductId}'";
            public const string DeleteFailed = "Failed to delete review with Id {ReviewId}";
            public const string DeleteSuccess = "Review with Id {ReviewId} deleted successfully.";
            public const string DeleteError = "Exception occurred while deleting review with Id {ReviewId}";
            public const string InvalidUserId = "Invalid or missing userId.";
        }

        public static class StripeWebhook
        {
            public const string WebhookReceived = "Stripe webhook received. Type={Type}";
            public const string ValidationFailed = "Stripe webhook signature validation failed.";
            public const string CheckoutNotPaid = "Checkout completed but not paid. SessionId={SessionId}, PaymentStatus={Status}";
            public const string MarkedAsPaid = "Order {OrderId} marked as paid by webhook.";
        }

        public static class Account
        {
            public const string InvalidUserId = "Invalid or missing userId.";
            public const string DeliveryDetailsUserNotFound = "Delivery details requested for non-existing user. UserId: {UserId}";
            public const string DeliveryDetailsUpdateSuccess = "Delivery details for user {UserId} updated successfully.";
            public const string DeliveryDetailsUpdateFailed = "Failed to update delivery details for user {UserId}";
        }

        public static class Category
        {
            public const string NotFound = "Category with Id {CategoryId} was not found.";
            public const string AddWithInvalidModelState = "Attempt by user {UserId} to add category with invalid model state.";
            public const string NameAlreadyExist = "Attempt to add category name that already exists - {CategoryName}";
            public const string AddFailed = "Failed to add category with name '{CategoryName}' by user {UserId}.";
            public const string AddSuccess = "Category '{CategoryName}' successfully added by user {UserId}";
            public const string AddError = "Exception occurred while adding category '{CategoryName}'";
            public const string RestoreInvalidCategory = "Restore attempt for non-existing category with Id {CategoryId} by user {UserId}";
            public const string RestoreFailed = "Failed to restore category with Id {CategoryId}";
            public const string RestoreSuccess = "Category with Id {CategoryId} was successfully restored by user {UserId}";
            public const string RestoreError = "Exception occurred while restoring category with Id {CategoryId}";
            public const string EditCategoryPageLoadError = "Error while preparing Edit category form for category with Id {CategoryId}";
            public const string EditWithInvalidModelState = "Attempt to edit category with Id {CategoryId} with invalid model state by user {UserId}";
            public const string EditFailed = "Failed to edit category '{CategoryName}'";
            public const string EditSuccess = "Category '{CategoryName}' successfully edited by user {UserId}";
            public const string EditError = "Exception occurred while editing category '{CategoryName}'";
            public const string DeleteCategoryPageLoadError = "Exception while preparing Delete category form for category with Id {CategoryId}";
            public const string DeleteWithInvalidModelState = "Attempt to delete category with Id {CategoryId} with invalid model state by user {UserId}";
            public const string DeleteFailed = "Failed to delete category with Id {CategoryId} by user {UserId}";
            public const string DeleteSuccess = "Category with Id {CategoryId} successfully deleted by user {UserId}";
            public const string DeleteError = "Exception occurred while deleting category with Id {CategoryId}";
        }

        public static class User
        {
            public const string InvalidUserId = "Invalid userId received: {UserId}";
            public const string RoleNotSelected = "Role not selected for user {UserId}";
            public const string RoleAssignSuccess = "Role '{Role}' assigned to user {UserId}";
            public const string RoleAssignFailed = "Failed to assign role '{Role}' to user {UserId}";
            public const string RoleAssignError = "Exception occurred while assigning role '{Role}' to user {UserId}";
        }
    }
}
