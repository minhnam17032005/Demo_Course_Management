using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Repositories
{
    public class UserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRange(List<UserRole> entities)
        {
            _context.UserRoles.AddRange(entities);
        }

        public void RemoveRange(List<UserRole> entities)
        {
            _context.UserRoles.RemoveRange(entities);
        }



    }
}
