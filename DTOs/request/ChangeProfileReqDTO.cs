using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.DTOs.request
{
    public class ChangeProfileReqDTO
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
    }
}
