using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using static TechStore.GCommon.ValidationConstants.User;

namespace TechStore.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity
             .Property(u => u.FullName)
                 .IsRequired()
                 .HasMaxLength(FullNameMaxLength);

            entity
                 .Property(u => u.Address)
                 .IsRequired()
                 .HasMaxLength(AddressMaxLength);

            entity
                .Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(u => u.IsDeleted == false);
        }
    }
}
