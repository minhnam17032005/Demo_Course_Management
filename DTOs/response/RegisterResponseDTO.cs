namespace ShopManagementAPI.DTOs.response
{
    public class RegisterResponseDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
