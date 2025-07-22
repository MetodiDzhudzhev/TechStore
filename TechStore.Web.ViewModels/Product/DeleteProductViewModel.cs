using System.ComponentModel.DataAnnotations;

namespace TechStore.Web.ViewModels.Product
{
    public class DeleteProductViewModel
    {
        [Required]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;


        [Required]
        public int CategoryId { get; set; }
    }
}
