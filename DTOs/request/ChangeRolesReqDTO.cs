using System.ComponentModel.DataAnnotations;

namespace Demo_Course_Management.DTOs.request
{
    public class ChangeRolesReqDTO
    {
        [Required]
        [MinLength(1)]
        public List<int> RoleIds { get; set; } = new();
    }
}
