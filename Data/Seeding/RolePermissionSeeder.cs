using Demo_Course_Management.Models.Enum;
using Demo_Course_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo_Course_Management.Data.Seeding
{
    public static class RolePermissionSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var roles = await context.Roles.ToListAsync();
            var permissions = await context.Permissions.ToListAsync();

            var admin = roles.First(x => x.Name == RoleType.ADMIN);
            var manager = roles.First(x => x.Name == RoleType.MANAGER);
            var staff = roles.First(x => x.Name == RoleType.STAFF);
            var customer = roles.First(x => x.Name == RoleType.CUSTOMER);

            var toAdd = new List<RolePermission>();

            void AddIfNotExists(int roleId, int permissionId)
            {
                if (!context.RolePermissions.Any(x => x.RoleId == roleId && x.PermissionId == permissionId))
                {
                    toAdd.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    });
                }
            }

            // ADMIN
            foreach (var p in permissions)
                AddIfNotExists(admin.Id, p.Id);

            // MANAGER
            foreach (var p in permissions.Where(p =>
                p.Module is "CATEGORY" or "PRODUCT" or "ORDER" or "USER"))
                AddIfNotExists(manager.Id, p.Id);

            // STAFF
            foreach (var p in permissions.Where(p =>
                p.Module == "CATEGORY"
                || p.Module == "PRODUCT"
                || (p.Module == "ORDER" &&
                    (p.Method == HttpMethodType.GET || p.Method == HttpMethodType.PATCH))
                || (p.Module == "USER" && p.Method == HttpMethodType.GET)))
                AddIfNotExists(staff.Id, p.Id);

            // CUSTOMER
            foreach (var p in permissions.Where(p =>
                (p.Module == "CATEGORY" && p.Method == HttpMethodType.GET)
                || (p.Module == "PRODUCT" && p.Method == HttpMethodType.GET)
                || (p.Module == "ORDER" &&
                    (p.Method == HttpMethodType.GET || p.Method == HttpMethodType.POST))))
                AddIfNotExists(customer.Id, p.Id);

            await context.RolePermissions.AddRangeAsync(toAdd);
            await context.SaveChangesAsync();
        }
    }
}
