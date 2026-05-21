using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.DTOs.request
{
    public class UpdateStockDTO
    {
        [Required]
        public int Stock { get; set; }
    }
}
