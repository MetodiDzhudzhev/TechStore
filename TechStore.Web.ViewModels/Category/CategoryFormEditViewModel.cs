using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Category;

namespace TechStore.Web.ViewModels.Category
{
    public class CategoryFormEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
