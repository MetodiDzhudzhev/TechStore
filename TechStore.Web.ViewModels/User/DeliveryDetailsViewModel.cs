using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.User;
using static TechStore.GCommon.ValidationConstants.Order;

namespace TechStore.Web.ViewModels.User
{
    public class DeliveryDetailsViewModel
    {
        [Required]
        [MinLength(FullNameMinLength)]
        [MaxLength(FullNameMaxLength)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MinLength(AddressMinLength)]
        [MaxLength(AddressMaxLength)]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;
    }
}
