using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to return rating details
    /// </summary>
    public class RatingResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DishId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Rate { get; set; }
        public string? Comment { get; set; }
    }

    public static class RatingExtension
    {
        public static RatingResponse ToRatingResponse(this Rating rating)
        {
            return new RatingResponse()
            {
                Id = rating.Id,
                UserId = rating.UserId,
                DishId = rating.DishId,
                OrderId = rating.OrderId,
                Rate = rating.Rate,
                Comment = rating.Comment,
            };
        }
    }
}
