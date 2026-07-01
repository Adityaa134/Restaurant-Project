namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Represents a cart item to merge into the user's cart.
    /// </summary>
    public class MergeCartRequest
    {
        public Guid DishId { get; set; }
        public int Quantity { get; set; }
    }
}
