using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.DTOs.request
{
    public class CreateOrderReqDTO
    {

        [Required]
        public List<CreateOrderItemDTO> Items { get; set; } = new();
    }
}
