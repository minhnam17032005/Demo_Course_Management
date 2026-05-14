using Demo_Course_Management.Models;
using Demo_Course_Management.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace Demo_Course_Management.Data.Seeding
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
