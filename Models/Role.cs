using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopManagementAPI.Models.Enum;


namespace ShopManagementAPI.Models
{
    [Table("Roles")]
    public class Role : BaseEntity
    {
        [Required]
        public RoleType Name { get; set; } // Chỉ ADMIN, STAFF, CUSTOMER

        [MaxLength(250)]
        public string? Description { get; set; }
        public List<UserRole> UserRoles { get; set; } = new();

        public List<RolePermission> RolePermissions { get; set; } = new();

    }
}
