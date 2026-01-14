using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restaurent.Core.Domain.Entities
{
    public class Dish
    {
        [Key] 
        public Guid DishId { get; set; }

        [StringLength(300)]
        public string DishName { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string Description { get; set; }

        [StringLength(1000)]
        public string Image_Path { get; set; }
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public ICollection<Carts> CartItems { get; set; } = new List<Carts>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
