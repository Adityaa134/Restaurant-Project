namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for JWT and refresh token information.
    /// </summary>
    public class TokenModel
    {
        public string? JwtToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
