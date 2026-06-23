using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to accept rating details from the client
    /// </summary>
    public class RatingRequest
    {
        [Required(ErrorMessage = "User Id can't be null")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Dish Id can't be null")]
        public Guid DishId { get; set; }

        [Required(ErrorMessage = "Order Id can't be null")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "Rating can't be null")]
        [Precision(18, 2)]
        public decimal Rate { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public Rating ToRating()
        {
            return new Rating()
            {
                UserId = UserId,
                DishId = DishId,
                OrderId = OrderId,
                Rate = Rate,
                Comment = Comment
            };
        }
    }
}
