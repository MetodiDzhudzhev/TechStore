using System.ComponentModel.DataAnnotations;

namespace TechStore.Web.ViewModels.Product
{
    public class DeleteProductViewModel
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;


        [Required]
        public int CategoryId { get; set; }
    }
}
