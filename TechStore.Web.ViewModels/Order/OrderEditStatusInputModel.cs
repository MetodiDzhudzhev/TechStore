using System.ComponentModel.DataAnnotations;
using TechStore.Data.Models.Enums;

namespace TechStore.Web.ViewModels.Order
{
    public class OrderEditStatusInputModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "New status")]
        public Status NewStatus { get; set; }
    }
}
