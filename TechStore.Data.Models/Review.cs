namespace TechStore.Data.Models
{
    public class Review
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
