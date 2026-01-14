namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for email content and placeholder values.
    /// </summary>
    public class EmailPlaceHolderDTO
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<KeyValuePair<string, string>> PlaceHolders { get; set; }
    }
}
