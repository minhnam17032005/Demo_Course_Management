using ShopManagementAPI.Models;
using ShopManagementAPI.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace ShopManagementAPI.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        { 
            try
            {
                Console.WriteLine("Seeding database...");

                await RoleSeeder.SeedAsync(context);
                Console.WriteLine("Roles seeded");

                await PermissionSeeder.SeedAsync(context);
                Console.WriteLine("Permissions seeded");

                await RolePermissionSeeder.SeedAsync(context);
                Console.WriteLine("RolePermissions seeded");

                await UserSeeder.SeedAsync(context);
                Console.WriteLine("Users seeded");

                Console.WriteLine("Seeding completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding failed: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"{ex.InnerException.Message}");

                throw;
            }
        }

    }
}
