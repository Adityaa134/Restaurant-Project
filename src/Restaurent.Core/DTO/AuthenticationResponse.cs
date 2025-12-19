using System;

namespace Restaurent.Core.DTO
{
    public class AuthenticationResponse
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime RefershTokenExpirationDateTime { get; set; }
        public string? Role { get; set; }
        public string? ProfileImage {  get; set; }
    }
}
