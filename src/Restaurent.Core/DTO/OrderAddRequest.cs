using System.ComponentModel.DataAnnotations;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to create the order  
    /// </summary>
    public class OrderAddRequest
    {
        [Required(ErrorMessage ="User Id can't be null")]
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "Order Date can't be null")]
        public DateTime OrderDate { get; set; }

        [Required]
        public List<OrderItemAddRequest> OrderItems { get; set; }
        [Required]
        public Guid DeliveryAddressId { get; set; }

        public Order ToOrder()
        {
            return new Order()
            {
                UserId = UserId.Value,
                OrderDate = OrderDate,
                OrderItems = OrderItems
                            .Select(item=>item.ToOrderItems())
                            .ToList(),
                DeliveryAddressId = DeliveryAddressId         
            };
        }
    }
}
