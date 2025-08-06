using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("User in the system")]
    public class User : IdentityUser<Guid>
    {
        [Comment("User's full name")]
        public string? FullName { get; set; }


        [Comment("User's address")]
        public string? Address { get; set; }


        [Comment("User's shopping cart")]
        public virtual Cart Cart { get; set; } = null!;


        [Comment("Shows if the user is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}
