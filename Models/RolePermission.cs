using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagementAPI.Models
{
    [Table("RolePermissions")]
    public class RolePermission
    {
        [Required]
        public int RoleId { get; set; } // FK → Role

        public Role Role { get; set; } = null!;

        [Required]
        public int PermissionId { get; set; } // FK → Permission

        public Permission Permission { get; set; } = null!;
    }
}
