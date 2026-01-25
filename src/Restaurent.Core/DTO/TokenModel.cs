namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for JWT and refresh token information.
    /// </summary>
    public class TokenModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefershTokenExpirationDateTime { get; set; }
    }
}
