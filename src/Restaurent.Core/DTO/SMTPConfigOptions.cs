namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for SMTP configuration settings.
    /// </summary>
    public class SMTPConfigOptions
    {
        public string Username { get; set; }
        public string SenderDisplayName { get; set; }
        public string SenderAddress { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public bool EnableSSL { get; set; }
        public bool UserDefaultCredentials { get; set; }
        public bool IsBodyHtml { get; set; }

    }
}
