using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Identity;

namespace Restaurent.Core.Domain.Entities
{
    [Index(nameof(DishId), nameof(OrderId), IsUnique = true)]
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DishId { get; set; }
        public Guid OrderId { get; set; }

        [Precision(18, 2)]
        public decimal Rate { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(DishId))]
        public Dish Dish { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
