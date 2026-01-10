using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.User;
using static TechStore.GCommon.ValidationConstants.Order;

namespace TechStore.Web.ViewModels.Order
{
    public class OrderDeliveryDetailsViewModel
    {
        [Required]
        [MaxLength(FullNameMaxLength)]
        [Display(Name = "Recipient name")]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(ShippingAddressMaxLength)]
        [Display(Name = "Shipping address")]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(EmailMaxLength)]
        [Display(Name = "Email address")]
        public string Email { get; set; } = null!;
    }
}
