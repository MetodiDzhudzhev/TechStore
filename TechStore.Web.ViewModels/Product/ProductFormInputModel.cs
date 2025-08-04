using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Product;

namespace TechStore.Web.ViewModels.Product
{
    public class ProductFormInputModel
    {

        public string? Id { get; set; }


        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;


        public string? ImageUrl { get; set; }


        [Required]
        [Range((double)PriceMinValue, (double)PriceMaxValue)]
        public decimal Price { get; set; }


        [Required]
        [Range(QuantityInStockMinValie, int.MaxValue)]
        public int QuantityInStock { get; set; }


        //If selected CategoryId does not exist or it is null -> first CategoryId from the list of categories will be taken.
        public int CategoryId { get; set; }
        public IEnumerable<AddProductCategoryDropDownModel>? Categories { get; set; }


        [Required]
        public int BrandId { get; set; }
        public IEnumerable<AddProductBrandDropDownModel>? Brands { get; set; }
    }
}
