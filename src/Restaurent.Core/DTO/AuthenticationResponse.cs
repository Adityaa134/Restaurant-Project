namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for authentication response details.
    /// </summary>
    public class AuthenticationResponse
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; } 
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? ProfileImage {  get; set; }
    }
}
