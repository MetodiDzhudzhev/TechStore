using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Order;

namespace TechStore.Web.ViewModels.Order
{
    public class OrderEditShippingDetailsInputModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(RecipientNameMaxLength)]
        [Display(Name = "Recipient")]
        public string RecipientName { get; set; } = null!;

        [Required]
        [MaxLength(ShippingAddressMaxLength)]
        [Display(Name = "Address")]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [Phone]
        [MaxLength(PhoneNumberMaxLength)]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(EmailMaxLength)]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
