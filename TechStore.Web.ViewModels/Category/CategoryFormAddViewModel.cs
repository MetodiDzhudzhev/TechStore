using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Category;
using static TechStore.GCommon.ValidationConstants.Shared;

namespace TechStore.Web.ViewModels.Category
{
    public class CategoryFormAddViewModel
    {
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Url]
        public string? ImageUrl { get; set; }
    }
}
