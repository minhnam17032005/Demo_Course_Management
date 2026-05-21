using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.DTOs.request
{
    public class CategoryRequestDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Tên danh mục

        [MaxLength(250)]
        public string? Description { get; set; } // Mô tả optional
    }
}
