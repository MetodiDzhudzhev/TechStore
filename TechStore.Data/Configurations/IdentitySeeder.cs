using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TechStore.Data.Models;

namespace TechStore.Data.Configurations
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole<Guid>> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            string[] roles = { "Manager", "Admin", "User" };

            foreach (var role in roles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    IdentityResult result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}");
                    }
                }
            }
        }

        private static async Task CreateUserWithRoleAsync(IServiceProvider serviceProvider, string email, string password, string role)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            User? user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = "Not specified",
                    Address = "Not specified"
                };

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user: {email}");
                }

                IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, role);

                if (!addRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to assign {role} role to user: {email}");
                }

                Cart cart = new Cart
                {
                    Id = user.Id,
                    User = user
                };

                dbContext.Carts.Add(cart);
                await dbContext.SaveChangesAsync();
            }
        }


        public static async Task SeedInitialUsersAsync(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            await CreateUserWithRoleAsync(serviceProvider, "manager@gmail.com", "Manager@123", "Manager");
            await CreateUserWithRoleAsync(serviceProvider, "admin@gmail.com", "Admin@123", "Admin");
            await CreateUserWithRoleAsync(serviceProvider, "user@gmail.com", "User@123", "User");
        }
    }
}
