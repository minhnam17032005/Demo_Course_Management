using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo_Course_Management.Models
{
        [Table("UserRoles")]
        public class UserRole : BaseEntity
        {
            [Required]
            public int UserId { get; set; }
            public User User { get; set; } = null!;

            [Required]
            public int RoleId { get; set; }
            public Role Role { get; set; } = null!;

    }
}
