using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Shared;

namespace TechStore.Web.ViewModels.Category
{
    public class DeleteCategoryViewModel
    {
        [Required]
        [Range(IntIdMinValue, IntIdMaxValue)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
