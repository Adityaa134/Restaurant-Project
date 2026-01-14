namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for pagination request parameters.
    /// </summary>
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
