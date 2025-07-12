using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;
using static TechStore.GCommon.ValidationConstants.Review;

namespace TechStore.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> entity)
        {
            entity
             .HasKey(r => r.Id);

            entity
                 .Property(r => r.Rating)
                 .IsRequired();

            entity
                 .Property(r => r.Comment)
                 .IsRequired(false)
                 .HasMaxLength(CommentMaxLength);

            entity
                 .Property(r => r.CreatedAt)
                 .IsRequired();

            entity
                .Property(r => r.IsDeleted)
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(r => r.IsDeleted == false
                                && r.Product.IsDeleted == false
                                && r.User.IsDeleted == false);

            entity
                 .HasOne(r => r.Product)
                 .WithMany(p => p.Reviews)
                 .HasForeignKey(r => r.ProductId)
                 .OnDelete(DeleteBehavior.NoAction);

            entity
                 .HasOne(r => r.User)
                 .WithMany(u => u.Reviews)
                 .HasForeignKey(r => r.UserId)
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
