using Demo_Course_Management.Data;
using Demo_Course_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo_Course_Management.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        // ================= ROLE =================
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

        // ================= PERMISSION =================
        // Lấy danh sách permission hợp lệ (tồn tại trong DB)
        public async Task<List<int>> GetValidPermissionIdsAsync(List<int> permissionIds)
        {
            return await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();
        }
        // Lấy danh sách permission đã tồn tại trong role
        public async Task<List<int>> GetExistingPermissionIdsAsync(int roleId, List<int> permissionIds)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .Select(rp => rp.PermissionId)
                .ToListAsync();
        }
        // Lấy các RolePermission entity để xóa
        public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .ToListAsync();
        }

        // ================= WRITE =================

        public void AddRolePermissions(List<RolePermission> entities)
        {
            _context.RolePermissions.AddRange(entities);
        }

        public void RemoveRolePermissions(List<RolePermission> entities)
        {
            _context.RolePermissions.RemoveRange(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
