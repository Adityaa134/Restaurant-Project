using System.ComponentModel.DataAnnotations;
using Restaurent.Core.Enums;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Represents a request to update the status of an order.
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public OrderStatus OrderStatus { get; set; }
    }
}
