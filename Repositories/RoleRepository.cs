using ShopManagementAPI.Data;
using ShopManagementAPI.Models;
using ShopManagementAPI.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace ShopManagementAPI.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Role>> GetAllWithPermissionsAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> FindByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role?> GetByNameAsync(RoleType roleType)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == roleType);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds)
        {
            return await _context.Roles
                .Where(x => roleIds.Contains(x.Id))
                .ToListAsync();
        }
    }
}
