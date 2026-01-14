using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to return items in the order 
    /// </summary>
    public class OrderItemResponse
    {
        public Guid DishId { get; set; }
        public string? DishName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public static class OrderItemExtension
    {
        public static OrderItemResponse ToOrderItemResponse(this OrderItem orderItem)
        {
            return new OrderItemResponse()
            {
                DishId = orderItem.DishId,
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity,
                DishName = orderItem.Dish?.DishName,
                TotalPrice = orderItem.Quantity * orderItem.UnitPrice
            };

        }
    }
}
