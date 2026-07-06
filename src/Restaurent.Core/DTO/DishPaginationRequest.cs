namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO for requesting dishes using cursor pagination.
    /// </summary>
    public class DishPaginationRequest
    {
        public DateTime? CursorCreatedAt { get; set; }
        public Guid? CursorDishId { get; set; }
        public int Take { get; set; } = 8;
    }
}
