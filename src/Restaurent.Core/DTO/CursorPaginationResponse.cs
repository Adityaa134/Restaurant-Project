namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for cursor based paginated response.
    /// </summary>
    public class CursorPaginationResponse<T>
    {
        public List<T> Items { get; set; }
        public bool HasMore { get; set; }
        public DateTime? NextCursorCreatedAt { get; set; }
        public Guid? NextCursorDishId { get; set; }
    }
}
