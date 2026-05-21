using ShopManagementAPI.Models.Enum;
using ShopManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ShopManagementAPI.Data.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var rolesToSeed = new List<Role>
            {
                new() { Name = RoleType.ADMIN, Description = "System Admin" },
                new() { Name = RoleType.MANAGER, Description = "Business Manager" },
                new() { Name = RoleType.STAFF, Description = "Staff" },
                new() { Name = RoleType.CUSTOMER, Description = "Customer" }
            };

            foreach (var role in rolesToSeed)
            {
                var exists = await context.Roles.AnyAsync(x => x.Name == role.Name);
                if (!exists)
                    await context.Roles.AddAsync(role);
            }

            await context.SaveChangesAsync();
        }
    }
}
