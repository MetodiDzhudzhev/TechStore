using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("Product review by a user")]
    public class Review
    {
        [Comment("Review identifier")]
        public long Id { get; set; }


        [Comment("Rating value")]
        public int Rating { get; set; }


        [Comment("Comment text")]
        public string? Comment { get; set; }


        [Comment("Date and time when the review was created")]
        public DateTime CreatedAt { get; set; }


        [Comment("Foreign key to the related Product")]
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;


        [Comment("Foreign key to the author of the review")]
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;


        [Comment("Shows if the review is deleted")]
        public bool IsDeleted { get; set; }
    }
}
