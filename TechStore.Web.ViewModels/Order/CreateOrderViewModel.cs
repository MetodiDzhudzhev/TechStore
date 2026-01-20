using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Order;

namespace TechStore.Web.ViewModels.Order
{
    public class CreateOrderViewModel
    {
        [Required]
        [MaxLength(RecipientNameMaxLength)]
        public string RecipientName { get; set; } = null!;

        [Required]
        [MaxLength(ShippingAddressMaxLength)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(EmailMaxLength)]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

}
