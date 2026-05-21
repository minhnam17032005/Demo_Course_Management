using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopManagementAPI.Models.Enum;

namespace ShopManagementAPI.Models
{
    [Table("Permissions")]
    public class Permission : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // CREATE_ORDER, VIEW_PRODUCT…

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(200)]
        public string ApiPath { get; set; } = null!; // /api/orders

        [Required]
        public HttpMethodType Method { get; set; } // enum GET, POST…

        [Required]
        [MaxLength(100)]
        public string Module { get; set; } = null!; // Orders, Products…

        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}
