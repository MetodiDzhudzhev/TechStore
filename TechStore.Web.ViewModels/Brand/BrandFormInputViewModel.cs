using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Brand;


namespace TechStore.Web.ViewModels.Brand
{
    public class BrandFormInputViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Url]
        public string? LogoUrl { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }
    }
}