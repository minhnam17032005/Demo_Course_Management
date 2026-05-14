using Demo_Course_Management.Models;
using Demo_Course_Management.Models.Enum;

namespace Demo_Course_Management.DTOs.response
{
    public class LoginResponseDTO
    {
        public UserInfo User { get; set; } = new();

        public string AccessToken { get; set; }

        public class UserInfo
        {
            public int Id { get; set; }

            public string Username { get; set; }

            public List<RoleItemDTO> Roles { get; set; } = new();
        }
    }
}
