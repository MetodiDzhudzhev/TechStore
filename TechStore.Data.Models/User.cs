using Microsoft.AspNetCore.Identity;

namespace TechStore.Data.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string Address { get; set; } = null!;

        public virtual Cart Cart { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

        public bool IsDeleted { get; set; }
    }
}
