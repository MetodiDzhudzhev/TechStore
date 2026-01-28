using System.ComponentModel.DataAnnotations;
using static TechStore.GCommon.ValidationConstants.Review;

namespace TechStore.Web.ViewModels.Review
{
    public class ReviewFormInputModel
    {
        public Guid ProductId { get; set; }

        [MaxLength(CommentMaxLength)]
        public string? Comment { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}
