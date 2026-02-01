using TechStore.Data.Models.Enums;

namespace TechStore.Web.ViewModels.Order
{
    public class OrderEditPageViewModel
    {
        public long Id { get; set; }
        public string OrderDate { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalSum { get; set; }
        public Status CurrentStatus { get; set; }

        public bool CanEditShipping { get; set; }

        public IReadOnlyCollection<Status> AllowedStatuses { get; set; } = Array.Empty<Status>();

        public OrderEditStatusInputModel Status { get; set; } = new();
        public OrderEditShippingDetailsInputModel Shipping { get; set; } = new();
    }
}
