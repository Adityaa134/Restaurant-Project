namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for user details
    /// </summary>
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
        public string? Role { get; set; }
    }
}
