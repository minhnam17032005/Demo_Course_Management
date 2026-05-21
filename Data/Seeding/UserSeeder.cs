using ShopManagementAPI.Middleware;
using ShopManagementAPI.Models;
using ShopManagementAPI.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace ShopManagementAPI.Data.Seeding
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var adminRole = await context.Roles.FirstAsync(x => x.Name == RoleType.ADMIN);

            var exists = await context.Users.AnyAsync(x => x.Username == "nam");
            if (exists) return;

            var user = new User
            {
                Username = "Nam",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "Nguyen Minh Nam",
                Email = "namcoder2005@gmail.com",
                IsActive = true,
                UserRoles = new List<UserRole>
        {
            new UserRole { RoleId = adminRole.Id }
        }
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
    }
}
