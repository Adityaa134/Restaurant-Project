using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.Enums;

namespace Restaurent.Core.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; }
        [Precision(18, 2)]
        public decimal TotalBill { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public List<OrderItem> OrderItems { get; set; } =  new List<OrderItem>();

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(DeliveryAddressId))]
        public Address DeliveryAddress { get; set; }
    }
}
