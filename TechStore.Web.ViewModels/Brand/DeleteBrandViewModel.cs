using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Shared;

namespace TechStore.Web.ViewModels.Brand
{
    public class DeleteBrandViewModel
    {
        [Required]
        [Range(IntIdMinValue, IntIdMaxValue)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
    }
}