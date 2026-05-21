using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.DTOs.request
{
    public class ChangeRolesReqDTO
    {
        [Required]
        [MinLength(1)]
        public List<int> RoleIds { get; set; } = new();
    }
}
