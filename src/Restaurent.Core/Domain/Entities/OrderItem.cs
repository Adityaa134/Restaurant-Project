using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restaurent.Core.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DishId { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [ForeignKey(nameof(DishId))]
        public Dish Dish {get;set;}
    }
}
