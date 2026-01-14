using System.ComponentModel.DataAnnotations;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to add items in order 
    /// </summary>
    public class OrderItemAddRequest
    {
        [Required(ErrorMessage = "Dish Id can't be null")]
        public Guid? DishId { get; set; }
        [Required(ErrorMessage ="Unit Price can't be null")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "Quanity can't be null")]
        public int Quantity { get; set; }

        public OrderItem ToOrderItems()
        {
            return new OrderItem()
            {
                DishId = DishId.Value,
                UnitPrice = UnitPrice,
                Quantity = Quantity,
            };
        }
    }
}
