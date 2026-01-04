using TechStore.Web.ViewModels.CartProduct;

namespace TechStore.Web.ViewModels.Cart
{
    public class CartViewModel
    {
        public IEnumerable<CartProductDetailsViewModel> Products { get; set; } = new List<CartProductDetailsViewModel>();

        public decimal TotalPrice => Products.Sum(p => p.Price * p.Quantity);
    }
}
