namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for paginated response data.
    /// </summary>
    /// <typeparam name="T">The type of items being paginated.</typeparam>
    public class PaginationResponse<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
