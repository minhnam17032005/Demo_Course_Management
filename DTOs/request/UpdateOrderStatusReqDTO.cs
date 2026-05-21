using System.ComponentModel.DataAnnotations;
using ShopManagementAPI.Models.Enum;

namespace ShopManagementAPI.DTOs.request
{
    public class UpdateOrderStatusReqDTO
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
