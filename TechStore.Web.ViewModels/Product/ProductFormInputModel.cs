using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Product;

namespace TechStore.Web.ViewModels.Product
{
    public class ProductFormInputModel
    {
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;


        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;


        public string? ImageUrl { get; set; }


        [Required]
        public decimal Price { get; set; }


        [Required]
        [Range(QuantityInStockMinValie, int.MaxValue)]
        public int QuantityInStock { get; set; }


        [Required]
        public int CategoryId { get; set; }
        public IEnumerable<AddProductCategoryDropDownModel>? Categories { get; set; }


        [Required]
        public int BrandId { get; set; }
        public IEnumerable<AddProductBrandDropDownModel>? Brands { get; set; }
    }
}
