using System.ComponentModel.DataAnnotations;

namespace Demo_Course_Management.DTOs.request
{
    public class ChangeProfileReqDTO
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
    }
}
